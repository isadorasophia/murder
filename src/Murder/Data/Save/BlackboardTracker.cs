using Bang;
using Bang.Entities;
using Murder.Assets;
using Murder.Core.Dialogs;
using Murder.Data;
using Murder.Diagnostics;
using Murder.Serialization;
using Newtonsoft.Json;
using System.Collections.Immutable;
using System.Reflection;

namespace Murder.Save
{
    /// <summary>
    /// Track variables that contain the state of the world.
    /// </summary>
    public class BlackboardTracker
    {
        [JsonProperty]
        private ImmutableDictionary<string, BlackboardInfo>? _blackboards;

        [JsonProperty]
        private readonly Dictionary<Guid, ImmutableDictionary<string, BlackboardInfo>> _characterBlackboards = new();

        [JsonProperty]
        private readonly ComplexDictionary<(Guid Character, int SituationId, int DialogId), int> _dialogCounter = new();

        [JsonProperty]
        private BlackboardInfo? _defaultBlackboard;

        [JsonIgnore]
        private Action? _onModified = () => { };

        private readonly Dictionary<Guid, Character> _characterCache = new();

        public virtual ImmutableDictionary<string, BlackboardInfo> FetchBlackboards() =>
            _blackboards ??= InitializeBlackboards();

        /// <summary>
        /// Fetch a cached character out of <see cref="_characterCache"/>
        /// </summary>
        /// <param name="guid">The asset guid for the character script.</param>
        /// <returns>Null if unable to find the asset for <paramref name="guid"/>.</returns>
        public Character? FetchCharacterFor(Guid guid)
        {
            if (!_characterCache.TryGetValue(guid, out Character? character))
            {
                if (Game.Data.TryGetAsset<CharacterAsset>(guid) is not CharacterAsset asset)
                {
                    GameLogger.Error("Unable to find character asset!");
                    throw new InvalidOperationException();
                }

                character = new(guid, asset.Situations);

                _characterCache[guid] = character;
            }

            return character;
        }

        /// <summary>
        /// Try to find a blackboard with name <paramref name="name"/>. 
        /// If this is specific to a character script, look under <paramref name="guid"/>.
        /// </summary>
        public virtual BlackboardInfo? FindBlackboard(string? name, Guid? guid)
        {
            _blackboards ??= InitializeBlackboards();

            if (name is null)
            {
                // On null names, return the default blackboard.
                return _defaultBlackboard;
            }

            if (_blackboards.TryGetValue(name, out var blackboard))
            {
                return blackboard;
            }

            if (guid is null)
            {
                GameLogger.Error($"Unable to find a blackboard for {name}.");
                return null;
            }

            // Otherwise, look for a character script blackboard.
            if (!_characterBlackboards.ContainsKey(guid.Value))
            {
                _characterBlackboards[guid.Value] = InitializeCharacterBlackboards();
            }

            if (_characterBlackboards[guid.Value].TryGetValue(name, out var speakerBlackboard))
            {
                return speakerBlackboard;
            }

            GameLogger.Error($"Unable to find a blackboard for {name}.");
            return null;
        }

        /// <summary>
        /// Track that a particular dialog option has been played.
        /// </summary>
        public virtual void Track(Guid character, int situationId, int dialogId)
        {
            var index = (character, situationId, dialogId);

            if (!_dialogCounter.ContainsKey(index))
            {
                _dialogCounter.Add(index, 0);
            }

            _dialogCounter[index]++;
        }

        /// <summary>
        /// Returns whether a particular dialog option has been played.
        /// </summary>
        public bool HasPlayed(Guid guid, int situationId, int dialogId)
        {
            return _dialogCounter.ContainsKey((guid, situationId, dialogId));
        }

        /// <summary>
        /// Returns whether how many times a dialog has been executed.
        /// </summary>
        public int PlayCount(Guid guid, int situationId, int dialogId)
        {
            if (_dialogCounter.TryGetValue((guid, situationId, dialogId), out int count))
            {
                return count;
            }

            return 0;
        }

        /// <summary>
        /// Get a blackboard value as a string. This returns the first blackboard that has the field.
        /// </summary>
        public string? GetValueAsString(string fieldName)
        {
            _blackboards ??= InitializeBlackboards();

            foreach ((Type tBlackboard, IBlackboard blackboard) in _blackboards.Values)
            {
                FieldInfo? f = tBlackboard.GetField(fieldName);
                if (f is not null)
                {
                    // Found our blackboard value!
                    return f.GetValue(blackboard)?.ToString();
                }
            }

            return null;
        }

        /// <summary>
        /// Return whether a <paramref name="fieldName"/> exists on <paramref name="blackboardName"/>.
        /// </summary>
        public bool HasVariable(string? blackboardName, string fieldName)
        {
            if (FindBlackboard(blackboardName, null) is not BlackboardInfo info)
            {
                // remove!
                return false;
            }

            FieldInfo? field = info.Type.GetField(fieldName);
            return field is not null;
        }

        public bool GetBool(string? name, string fieldName, Guid? character = null)
        {
            if (FindBlackboard(name, character) is not BlackboardInfo info)
            {
                GameLogger.Warning($"Unable to find the bool variable '{name}.{fieldName}' in blackboard.");
                return false;
            }

            return GetValue<bool>(info, fieldName);
        }

        public void SetBool(string? name, string fieldName, bool value, Guid? character = null)
        {
            if (FindBlackboard(name, character) is not BlackboardInfo info)
            {
                GameLogger.Warning($"Unable to find and set the variable '{name}.{fieldName}' to '{value}' in blackboard.");
                return;
            }

            SetValue(info, fieldName, value);
        }

        public int GetInt(string? name, string fieldName, Guid? character = null)
        {
            if (FindBlackboard(name, character) is not BlackboardInfo info)
            {
                GameLogger.Warning($"Unable to find the int variable '{name}.{fieldName}' in blackboard.");
                return 0;
            }

            return GetValue<int>(info, fieldName);
        }

        public void SetInt(string? name, string fieldName, BlackboardActionKind kind, int value, Guid? character = null)
        {
            if (FindBlackboard(name, character) is not BlackboardInfo info)
            {
                GameLogger.Warning($"Unable to find and set the variable '{name}.{fieldName}' to '{value}' in blackboard.");
                return;
            }

            FieldInfo? f = GetFieldFrom(info.Type, fieldName);
            if (f is null)
            {
                return;
            }

            GameLogger.Verify(f.FieldType == typeof(int), "Wrong type for dialog variable!");

            IBlackboard blackboard = info.Blackboard;
            int originalValue = (int)f.GetValue(blackboard)!;

            switch (kind)
            {
                case BlackboardActionKind.Add:
                    f.SetValue(blackboard, originalValue + value);
                    break;

                case BlackboardActionKind.Minus:
                    f.SetValue(blackboard, originalValue - value);
                    break;

                case BlackboardActionKind.Set:
                    f.SetValue(blackboard, value);
                    break;

                case BlackboardActionKind.SetMax:
                    f.SetValue(blackboard, Math.Max(value, originalValue));
                    break;
                    
                case BlackboardActionKind.SetMin:
                    f.SetValue(blackboard, Math.Min(value, originalValue));
                    break;
            }

            OnModified();
        }

        public string GetString(string? name, string fieldName, Guid? character = null)
        {
            if (FindBlackboard(name, character) is not BlackboardInfo info)
            {
                GameLogger.Warning($"Unable to find the variable '{name}.{fieldName}' in blackboard.");
                return string.Empty;
            }

            return GetValue<string>(info, fieldName);
        }

        public void SetString(string? name, string fieldName, string value, Guid? character = null)
        {
            if (FindBlackboard(name, character) is not BlackboardInfo info)
            {
                GameLogger.Warning($"Unable to find and set the variable '{name}.{fieldName}' to '{value}' in blackboard.");
                return;
            }

            SetValue(info, fieldName, value);
        }

        private T GetValue<T>(BlackboardInfo info, string fieldName)
        {
            FieldInfo? f = GetFieldFrom(info.Type, fieldName);
            if (f is null)
            {
                return default!;
            }

            GameLogger.Verify(f.FieldType == typeof(T), "Wrong type for dialog variable!");

            return (T)f.GetValue(info.Blackboard)!;
        }

        private bool SetValue<T>(BlackboardInfo info, string fieldName, T value)
        {
            FieldInfo? f = GetFieldFrom(info.Type, fieldName);
            if (f is null)
            {
                return false;
            }

            GameLogger.Verify(f.FieldType == typeof(T), "Wrong type for dialog variable!");

            f.SetValue(info.Blackboard, value);
            OnModified();

            return true;
        }

        /// <summary>
        /// Notify that the blackboard has been changed (externally or internally).
        /// </summary>
        public void OnModified()
        {
            _onModified?.Invoke();
        }

        /// <summary>
        /// This will watch any chages to any of the blackboard properties.
        /// </summary>
        public void Watch(Action notification)
        {
            _onModified += notification;
        }

        /// <summary>
        /// This will reset all watchers of trackers.
        /// </summary>
        public void ResetWatchers()
        {
            _onModified = null;
        }

        /// <summary>
        /// Returns whether a <paramref name="criterion"/> matches the current state of the blackboard and
        /// its score.
        /// </summary>
        /// <param name="criterion">The criterion to be matched.</param>
        /// <param name="character">This is used when checking for a particular character blackboard.</param>
        /// <param name="world">World.</param>
        /// <param name="entityId">Entity which will be used to check for components requirements.</param>
        /// <param name="weight">The weight of this match. Zero if there is a no match.</param>
        public bool Matches(Criterion criterion, Guid? character, World world, int? entityId, out int weight)
        {
            weight = 1;

            if (FindBlackboard(criterion.Fact.Blackboard, character) is not BlackboardInfo info)
            {
                // remove!
                throw new NotImplementedException();
            }

            switch (criterion.Fact.Kind)
            {
                case FactKind.Bool:
                    bool @bool = GetValue<bool>(info, fieldName: criterion.Fact.Name);
                    if (criterion.Kind is CriterionKind.Is)
                    {
                        return criterion.BoolValue == @bool;
                    }
                    else if (criterion.Kind is CriterionKind.Different)
                    {
                        return criterion.BoolValue != @bool;
                    }

                    break;

                case FactKind.Int:
                    int @int = GetValue<int>(info, fieldName: criterion.Fact.Name);
                    switch (criterion.Kind)
                    {
                        case CriterionKind.Less:
                            return @int < criterion.IntValue;

                        case CriterionKind.LessOrEqual:
                            return @int <= criterion.IntValue;

                        case CriterionKind.Is:
                            return @int == criterion.IntValue;
                        
                        case CriterionKind.Different:
                            return @int != criterion.IntValue;

                        case CriterionKind.BiggerOrEqual:
                            return @int >= criterion.IntValue;

                        case CriterionKind.Bigger:
                            return @int > criterion.IntValue;
                    }

                    break;

                case FactKind.String:
                    string @string = GetValue<string>(info, fieldName: criterion.Fact.Name);
                    if (criterion.Kind is CriterionKind.Is)
                    {
                        return string.Equals(criterion.StrValue, @string);
                    }
                    else if (criterion.Kind is CriterionKind.Different)
                    {
                        return !string.Equals(criterion.StrValue, @string);
                    }

                    break;
                    
                case FactKind.Weight:
                    weight = criterion.IntValue!.Value;

                    // Automatic match!
                    return true;
                    
                case FactKind.Component:
                    if (criterion.Fact.ComponentType is Type componentTarget && 
                        entityId is not null &&
                        world.TryGetEntity(entityId.Value) is Entity target)
                    {
                        bool hasComponent = target.HasComponent(componentTarget);
                        return hasComponent == criterion.BoolValue;
                    }
                     
                    break;
            }

            weight = 0;
            return false;
        }

        private FieldInfo? GetFieldFrom(Type type, string fieldName)
        {
            FieldInfo? f = type.GetField(fieldName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (f is null)
            {
                GameLogger.Fail($"Unable to find field for {fieldName}.");
                return null;
            }

            return f;
        }

        private IEnumerable<Type> FindAllBlackboards(Type tInterface, Type? tFilterOut = null)
        {
            bool isBlackboard(Type t)
            {
                if (t.IsInterface || t.IsAbstract)
                {
                    return false;
                }

                if (tFilterOut != null && tFilterOut.IsAssignableFrom(t))
                {
                    return false;
                }

                return Attribute.IsDefined(t, typeof(BlackboardAttribute)) && tInterface.IsAssignableFrom(t);
            }

            Assembly[] allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly s in allAssemblies)
            {
                foreach (Type t in s.GetTypes())
                {
                    if (isBlackboard(t))
                    {
                        yield return t;
                    }
                }
            }
        }

        private ImmutableDictionary<string, BlackboardInfo> InitializeBlackboards()
        {
            var result = ImmutableDictionary.CreateBuilder<string, BlackboardInfo>();
            foreach (Type t in FindAllBlackboards(typeof(IBlackboard), typeof(ICharacterBlackboard)))
            {
                BlackboardAttribute attribute = t.GetCustomAttribute<BlackboardAttribute>()!;

                IBlackboard blackboard = (IBlackboard)Activator.CreateInstance(t)!;

                BlackboardInfo info = new(t, blackboard);
                if (attribute.IsDefault)
                {
                    _defaultBlackboard = info;
                }

                result.Add(attribute.Name, info);
            }

            GameLogger.Verify(_defaultBlackboard is not null, "Unable to find a default blackboard.");
            return result.ToImmutable();
        }

        private ImmutableArray<Type>? _cachedCharacterBlackboards = null;
        
        private ImmutableDictionary<string, BlackboardInfo> InitializeCharacterBlackboards()
        {
            _cachedCharacterBlackboards ??= FindAllBlackboards(typeof(ICharacterBlackboard)).ToImmutableArray();
            
            var result = ImmutableDictionary.CreateBuilder<string, BlackboardInfo>();
            foreach (Type t in _cachedCharacterBlackboards)
            {
                string name = t.GetCustomAttribute<BlackboardAttribute>()!.Name;
                result.Add(name, new(t, (ICharacterBlackboard)Activator.CreateInstance(t)!));
            }

            return result.ToImmutable();
        }
    }
}
