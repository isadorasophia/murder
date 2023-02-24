using Bang;
using Bang.Entities;
using Murder.Core.Dialogs;
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
        private ImmutableDictionary<string, (Type t, IBlackboard blackboard)>? _blackboards;

        [JsonProperty]
        private readonly Dictionary<Guid, ImmutableDictionary<string, (Type t, ICharacterBlackboard blackboard)>> _characterBlackboards = new();

        [JsonProperty]
        private readonly ComplexDictionary<(Guid Character, int SituationId, int DialogId), int> _dialogCounter = new();

        [JsonIgnore]
        private Action? _onModified = () => { };

        public virtual ImmutableDictionary<string, (Type t, IBlackboard blackboard)> FetchBlackboards() =>
            _blackboards ??= InitializeBlackboards();

        public virtual (Type t, object blackboard) FindBlackboard(string name, Guid? guid)
        {
            _blackboards ??= InitializeBlackboards();

            if (_blackboards.TryGetValue(name, out var blackboard))
            {
                return blackboard;
            }

            if (guid is null)
            {
                GameLogger.Error($"Unable to find a blackboard for {name}.");
                throw new InvalidOperationException();
            }

            // Otherwise, look for character blackboard.
            if (!_characterBlackboards.ContainsKey(guid.Value))
            {
                _characterBlackboards[guid.Value] = InitializeCharacterBlackboards();
            }

            if (_characterBlackboards[guid.Value].TryGetValue(name, out var speakerBlackboard))
            {
                return speakerBlackboard;
            }

            GameLogger.Error($"Unable to find a blackboard for {name}.");
            throw new InvalidOperationException();
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
        public bool HasVariable(string blackboardName, string fieldName)
        {
            (Type type, object blackboard) = FindBlackboard(blackboardName, null);
            FieldInfo? field = type.GetField(fieldName);

            return field is not null;
        }

        public bool GetBool(string name, string fieldName, Guid? character = null)
        {
            (Type type, object blackboard) = FindBlackboard(name, character);

            FieldInfo f = GetFieldFrom(type, fieldName);
            GameLogger.Verify(f.FieldType == typeof(bool), "Wrong type for dialog variable!");

            return (bool)f.GetValue(blackboard)!;
        }

        public void SetBool(string name, string fieldName, bool value, Guid? character = null)
        {
            (Type type, object blackboard) = FindBlackboard(name, character);

            FieldInfo f = GetFieldFrom(type, fieldName);
            GameLogger.Verify(f.FieldType == typeof(bool), "Wrong type for dialog variable!");

            f.SetValue(blackboard, value);

            _onModified?.Invoke();
        }

        public int GetInt(string name, string fieldName, Guid? character = null)
        {
            (Type type, object blackboard) = FindBlackboard(name, character);

            FieldInfo f = GetFieldFrom(type, fieldName);
            GameLogger.Verify(f.FieldType == typeof(int), "Wrong type for dialog variable!");

            return (int)f.GetValue(blackboard)!;
        }

        public void SetInt(string name, string fieldName, BlackboardActionKind kind, int value, Guid? character = null)
        {
            (Type type, object blackboard) = FindBlackboard(name, character);

            FieldInfo f = GetFieldFrom(type, fieldName);
            GameLogger.Verify(f.FieldType == typeof(int), "Wrong type for dialog variable!");

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
            }

            _onModified?.Invoke();
        }

        public string GetString(string name, string fieldName, Guid? character = null)
        {
            (Type type, object blackboard) = FindBlackboard(name, character);

            FieldInfo f = GetFieldFrom(type, fieldName);
            GameLogger.Verify(f.FieldType == typeof(string), "Wrong type for dialog variable!");

            return (string)f.GetValue(blackboard)!;
        }

        public void SetString(string name, string fieldName, string value, Guid? character = null)
        {
            (Type type, object blackboard) = FindBlackboard(name, character);

            FieldInfo f = GetFieldFrom(type, fieldName);
            GameLogger.Verify(f.FieldType == typeof(string), "Wrong type for dialog variable!");

            f.SetValue(blackboard, value);

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
            
            switch (criterion.Fact.Kind)
            {
                case FactKind.Bool:
                    bool @bool = GetBool(criterion.Fact.Blackboard, criterion.Fact.Name, character);
                    if (criterion.Kind is CriterionKind.Is)
                    {
                        return criterion.BoolValue == @bool;
                    }

                    break;

                case FactKind.Int:
                    int @int = GetInt(criterion.Fact.Blackboard, criterion.Fact.Name, character);
                    switch (criterion.Kind)
                    {
                        case CriterionKind.Less:
                            return @int < criterion.IntValue;

                        case CriterionKind.LessOrEqual:
                            return @int <= criterion.IntValue;

                        case CriterionKind.Equal:
                            return @int == criterion.IntValue;

                        case CriterionKind.BiggerOrEqual:
                            return @int >= criterion.IntValue;

                        case CriterionKind.Bigger:
                            return @int > criterion.IntValue;
                    }

                    break;

                case FactKind.String:
                    string @string = GetString(criterion.Fact.Blackboard, criterion.Fact.Name, character);
                    if (criterion.Kind is CriterionKind.Matches)
                    {
                        return criterion.StrValue == @string;
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

        private FieldInfo GetFieldFrom(Type type, string fieldName)
        {
            FieldInfo? f = type.GetField(fieldName);
            if (f is null)
            {
                GameLogger.Fail($"Unable to acquire field for {fieldName}.");
                throw new InvalidOperationException();
            }

            return f;
        }

        private IEnumerable<Type> FindAllBlackboards(Type tInterface)
        {
            var isBlackboard = (Type t) => !t.IsInterface && !t.IsAbstract && tInterface.IsAssignableFrom(t) &&
                Attribute.IsDefined(t, typeof(BlackboardAttribute));

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

        private ImmutableDictionary<string, (Type t, IBlackboard blackboard)> InitializeBlackboards()
        {
            var result = ImmutableDictionary.CreateBuilder<string, (Type t, IBlackboard blackboard)>();
            foreach (Type t in FindAllBlackboards(typeof(IBlackboard)))
            {
                string name = t.GetCustomAttribute<BlackboardAttribute>()!.Name;
                result.Add(name, (t, (IBlackboard)Activator.CreateInstance(t)!));
            }

            return result.ToImmutable();
        }

        private ImmutableArray<Type>? _cachedCharacterBlackboards = null;
        
        private ImmutableDictionary<string, (Type t, ICharacterBlackboard blackboard)> InitializeCharacterBlackboards()
        {
            _cachedCharacterBlackboards ??= FindAllBlackboards(typeof(ICharacterBlackboard)).ToImmutableArray();
            
            var result = ImmutableDictionary.CreateBuilder<string, (Type t, ICharacterBlackboard blackboard)>();
            foreach (Type t in _cachedCharacterBlackboards)
            {
                string name = t.GetCustomAttribute<BlackboardAttribute>()!.Name;
                result.Add(name, (t, (ICharacterBlackboard)Activator.CreateInstance(t)!));
            }

            return result.ToImmutable();
        }
    }
}
