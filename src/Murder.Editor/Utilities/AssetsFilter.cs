using Bang.Components;
using Bang.Interactions;
using Bang.StateMachines;
using Murder.Assets;
using Murder.Attributes;
using Murder.Core;
using Murder.Core.Dialogs;
using Murder.Core.Graphics;
using Murder.Core.Physics;
using Murder.Core.Sounds;
using Murder.Diagnostics;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Murder.Editor.Utilities
{
    internal static class AssetsFilter
    {
        private readonly static Lazy<ImmutableArray<(string name, int id)>> _collisionLayers = new(() =>
        {
            var layers = new List<(string name, int id)>();
            foreach (var type in ReflectionHelper.GetAllImplementationsOf<CollisionLayersBase>())
            {
                var constants = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
                foreach (var constant in constants)
                {
                    var item = (constant.Name, (int)constant.GetValue(null)!);
                    if (layers.Contains(item))
                        continue;

                    layers.Add(item);
                }
            }

            return layers.ToImmutableArray();
        });

        private readonly static Lazy<ImmutableArray<(string name, int id)>> _tags = new(() =>
        {
            var layers = new List<(string name, int id)>();
            foreach (var type in ReflectionHelper.GetAllImplementationsOf<MurderTagsBase>())
            {
                var constants = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
                foreach (var constant in constants)
                {
                    var item = (constant.Name, (int)constant.GetValue(null)!);
                    if (layers.Contains(item))
                        continue;

                    layers.Add(item);
                }
            }

            return layers.ToImmutableArray();
        });

        // Sprite Batches
        private readonly static Lazy<ImmutableArray<(string name, int id)>> _spriteBatches = new(() =>
        {
            var spriteBatches = new List<(string name, int id)>();
            foreach (var type in ReflectionHelper.GetAllImplementationsOf<Batches2D>())
            {
                var constants = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();

                foreach (var constant in constants)
                {
                    var itemName = constant.Name;
                    if (itemName.EndsWith("batch", StringComparison.OrdinalIgnoreCase))
                    {
                        itemName = itemName.Substring(0, itemName.Length - "batch".Length);
                    }
                    else if (itemName.EndsWith("batchId", StringComparison.OrdinalIgnoreCase))
                    {
                        itemName = itemName.Substring(0, itemName.Length - "batchId".Length);
                    }

                    var item = (itemName, (int)constant.GetValue(null)!);
                    if (spriteBatches.Contains(item))
                        continue;

                    spriteBatches.Add(item);
                }
            }

            return spriteBatches.ToImmutableArray();
        });

        private readonly static Lazy<string[]> _spriteBatchesNames = new(() =>
        {
            return SpriteBatches.Select(item => Prettify.FormatVariableName(item.name)).ToArray();
        });
        public static ImmutableArray<(string name, int id)> SpriteBatches => _spriteBatches.Value;
        public static string[] SpriteBatchesNames => _spriteBatchesNames.Value;

        // Fonts
        private readonly static Lazy<ImmutableArray<(string name, int id)>> _fonts = new(() =>
        {
            var fonts = new List<(string name, int id)>();
            foreach (var type in ReflectionHelper.GetEnumsWithAttribute(typeof(FontAttribute)))
            {
                var constants = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();

                foreach (var constant in constants)
                {
                    var itemName = constant.Name;
                    var item = (itemName, (int)constant.GetValue(null)!);
                    if (fonts.Contains(item))
                        continue;

                    fonts.Add(item);
                }
            }

            return fonts.ToImmutableArray();
        });

        private readonly static Lazy<string[]> _fontNames = new(() =>
        {
            return Fonts.Select(item => Prettify.FormatVariableName(item.name)).ToArray();
        });
        public static ImmutableArray<(string name, int id)> Fonts => _fonts.Value;
        public static string[] FontNames => _fontNames.Value;


        private readonly static Lazy<string[]> _collisionLayersNames = new(() =>
        {
            return CollisionLayers.Select(item => Prettify.CapitalizeFirstLetter(item.name)).ToArray();
        });

        private readonly static Lazy<string[]> _tagsNames = new(() =>
        {
            return Tags.Select(item => Prettify.CapitalizeFirstLetter(item.name)).ToArray();
        });

        public static ImmutableArray<(string name, int id)> CollisionLayers => _collisionLayers.Value;
        public static ImmutableArray<(string name, int id)> Tags => _tags.Value;
        public static string[] CollisionLayersNames => _collisionLayersNames.Value;
        public static string[] TagsNames => _tagsNames.Value;

        private static readonly Lazy<ImmutableArray<Type>> _componentTypes = new(() =>
        {
            return ReflectionHelper.GetAllImplementationsOf<IComponent>()
                .Where(t => !Attribute.IsDefined(t, typeof(HideInEditorAttribute))
                    && !typeof(IMessage).IsAssignableFrom(t)
                    && !Attribute.IsDefined(t, typeof(RuntimeOnlyAttribute))
                    && !Attribute.IsDefined(t, typeof(HideInEditorAttribute)))
                .ToImmutableArray();
        });

        public static ImmutableArray<Type> GetAllComponents() => _componentTypes.Value;

        private static readonly Lazy<ImmutableArray<Type>> _stateMachines = new(() =>
        {
            return ReflectionHelper.GetAllImplementationsOf<StateMachine>()
                .Where(t => !Attribute.IsDefined(t, typeof(RuntimeOnlyAttribute))
                    && !Attribute.IsDefined(t, typeof(HideInEditorAttribute)))
                .ToImmutableArray();
        });

        public static ImmutableArray<Type> GetAllStateMachines() => _stateMachines.Value;

        private static readonly Lazy<ImmutableArray<Type>> _iteractions = new(() =>
        {
            return ReflectionHelper.GetAllImplementationsOf<IInteraction>()
                .ToImmutableArray();
        });

        public static ImmutableArray<Type> GetAllInteractions() => _iteractions.Value;

        public static IEnumerable<PrefabAsset> GetAllCandidatePrefabs()
        {
            return Architect.EditorData.FilterAllAssets(typeof(PrefabAsset)).Values
                .Select(e => (PrefabAsset)e);
        }

        public static IEnumerable<Type> GetAllSystems()
        {
            return ReflectionHelper.SafeGetAllTypesInAllAssemblies()
                .Where(type => type.GetInterfaces().Contains(typeof(Bang.Systems.ISystem)) && !type.IsInterface);
        }

        public static IEnumerable<Type> GetFromInterface(Type @interface)
        {
            return ReflectionHelper.SafeGetAllTypesInAllAssemblies()
                .Where(type => type.GetInterfaces().Contains(@interface) && !type.IsInterface);
        }

        private static Dictionary<string, Fact>? _blackboards = null;
        private static Dictionary<string /* fact name */, Type /* type */>? _factsToType = null;

        private static Dictionary<string, SoundFact>? _soundBlackboards = null;
        private static Dictionary<BlackboardKind, Dictionary<string, Fact>>? _kindToBlackboards = null;

        public static void RefreshCache()
        {
            _blackboards = null;
            _factsToType = null;

            _soundBlackboards = null;
            _soundBlackboardsTypes = null;
        }

        private static ImmutableArray<Type>? _soundBlackboardsTypes;

        public static ImmutableArray<Type> FetchAllSoundBlackboards()
        {
            if (_soundBlackboardsTypes is not null)
            {
                return _soundBlackboardsTypes.Value;
            }

            _soundBlackboardsTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(ISoundBlackboard).IsAssignableFrom(type))
                .Where(t => Attribute.IsDefined(t, typeof(BlackboardAttribute)))
                .ToImmutableArray();

            return _soundBlackboardsTypes.Value;
        }

        public static Dictionary<string, SoundFact> GetAllFactsFromSoundBlackboards() =>
            _soundBlackboards ??= FetchAllFactsFromSoundBlackboards();

        private static Dictionary<string, SoundFact> FetchAllFactsFromSoundBlackboards()
        {
            var facts = ImmutableArray.CreateBuilder<SoundFact>();
            foreach (Type t in FetchAllSoundBlackboards())
            {
                BlackboardAttribute blackboard = (BlackboardAttribute)
                    Attribute.GetCustomAttribute(t, typeof(BlackboardAttribute))!;

                foreach (FieldInfo field in t.GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    SoundFact fact = new(blackboard.Name, field.Name);
                    facts.Add(fact);
                }
            }

            return facts.ToDictionary(f => f.Name, f => f, StringComparer.OrdinalIgnoreCase);
        }

        public static Dictionary<string, Fact> GetAllFactsFromBlackboards(BlackboardKind kind = BlackboardKind.All)
        {
            if (_blackboards is null || _kindToBlackboards is null)
            {
                InitializeFactsFromBlackboards();
            }

            if (kind != BlackboardKind.All)
            {
                if (!_kindToBlackboards.TryGetValue(kind, out Dictionary<string, Fact>? result))
                {
                    return [];
                }

                return result;
            }

            return _blackboards;
        }

        public static Type? FetchTypeForFact(string fact)
        {
            if (_factsToType is null)
            {
                InitializeFactsFromBlackboards();
            }

            if (string.IsNullOrWhiteSpace(fact))
            {
                return null;
            }

            _factsToType.TryGetValue(fact, out Type? value);
            return value;
        }

        [MemberNotNull(nameof(_blackboards))]
        [MemberNotNull(nameof(_factsToType))]
        [MemberNotNull(nameof(_kindToBlackboards))]
        private static void InitializeFactsFromBlackboards()
        {
            IEnumerable<Type> blackboardTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => Attribute.IsDefined(t, typeof(BlackboardAttribute)));

            Dictionary<string, Type> factNameToType = new(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, Fact> factNameToFact = new(StringComparer.OrdinalIgnoreCase);
            Dictionary<BlackboardKind, Dictionary<string, Fact>> kindToBlackboards = new();

            foreach (Type t in blackboardTypes)
            {
                BlackboardAttribute blackboard = (BlackboardAttribute)Attribute.GetCustomAttribute(t, typeof(BlackboardAttribute))!;

                Dictionary<string, Fact>? factNameToFactForKind = null;
                if (Activator.CreateInstance(t) is IBlackboard blackboardInstance)
                {
                    if (!kindToBlackboards.TryGetValue(blackboardInstance.Kind, out factNameToFactForKind))
                    {
                        factNameToFactForKind = new(StringComparer.OrdinalIgnoreCase);
                        kindToBlackboards[blackboardInstance.Kind] = factNameToFactForKind;
                    }
                }

                foreach (FieldInfo field in t.GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    Type fieldType = field.FieldType;

                    FactKind kind = FactKind.Invalid;
                    if (fieldType == typeof(string))
                    {
                        kind = FactKind.String;
                    }
                    else if (fieldType == typeof(bool))
                    {
                        kind = FactKind.Bool;
                    }
                    else if (fieldType == typeof(int))
                    {
                        kind = FactKind.Int;
                    }
                    else if (fieldType == typeof(float))
                    {
                        kind = FactKind.Float;
                    }
                    else if (fieldType.IsAssignableTo(typeof(Enum)))
                    {
                        kind = FactKind.Enum;
                    }
                    else
                    {
                        kind = FactKind.Any;
                    }

                    Fact fact = new(blackboard.Name, field.Name, kind);

                    factNameToFact[fact.EditorName] = fact;
                    factNameToType[fact.EditorName] = fieldType;

                    if (factNameToFactForKind is not null)
                    {
                        factNameToFactForKind[fact.EditorName] = fact;
                    }
                }
            }

            _blackboards = factNameToFact;
            _factsToType = factNameToType;
            _kindToBlackboards = kindToBlackboards;
        }

        private static readonly Lazy<ImmutableDictionary<string, Type>> _allComponentsByName = new(() =>
        {
            Dictionary<string, Type> candidates = GetAllComponents()
                .Where(t => !t.IsGenericType)
                .ToDictionary(t => t.Name, t => t, StringComparer.OrdinalIgnoreCase);

            FetchStateMachines(candidates);
            FetchInteractions(candidates);

            return candidates.ToImmutableDictionary();
        });

        public static Type? FindComponentWithName(string name)
        {
            if (_allComponentsByName.Value.TryGetValue(name, out Type? t))
            {
                return t;
            }

            return null;
        }

        /// <summary>
        /// Add types for the state machine components (generic).
        /// </summary>
        /// <param name="candidates">List of candidates returned.</param>
        /// <param name="inheritFrom">Optional state machine subtype.</param>
        /// <param name="candidates">Exclude components of these types.</param>
        public static void FetchStateMachines(
            Dictionary<string, Type> candidates,
            Type? subtypeOf = null,
            IEnumerable<IComponent>? excludeComponents = default)
        {
            if (excludeComponents?.FirstOrDefault(t => t is IStateMachineComponent) is not null)
            {
                // We already have a state machine, just go away.
                return;
            }

            Type tStateMachine = typeof(StateMachineComponent<>);

            if (subtypeOf is not null)
            {
                IEnumerable<Type> subStateMachines = ReflectionHelper.GetAllImplementationsOf(subtypeOf)
                    .Where(t => t != subtypeOf &&
                                !Attribute.IsDefined(t, typeof(RuntimeOnlyAttribute)) && 
                                !Attribute.IsDefined(t, typeof(HideInEditorAttribute)));

                foreach (var t in subStateMachines)
                {
                    candidates[t.Name] = tStateMachine.MakeGenericType(t);
                }

                return;
            }

            foreach (var t in GetAllStateMachines())
            {
                candidates[t.Name] = tStateMachine.MakeGenericType(t);
            }
        }

        /// <summary>
        /// Add types for the interaction components (generic).
        /// </summary>
        public static void FetchInteractions(
            Dictionary<string, Type> candidates,
            IEnumerable<IComponent>? excludeComponents = default)
        {
            if (excludeComponents?.FirstOrDefault(t => t is IInteractiveComponent) is not null)
            {
                // We already have a state machine, just go away.
                return;
            }

            Type? tInteraction = ReflectionHelper.TryFindType("Bang.Interactions.InteractiveComponent`1");
            if (tInteraction is null)
            {
                GameLogger.Fail("Could not find the state machine component for adding a new component?");
                return;
            }

            foreach (var t in GetAllInteractions())
            {
                candidates[t.Name] = tInteraction.MakeGenericType(t);
            }
        }

        public static string GetValidName(Type t, string name, int depth = 0)
        {
            ImmutableHashSet<string> names = Game.Data.FindAllNamesForAsset(t);
            if (names.Contains(name))
            {
                if (Regex.Match(name, "([0-9]+)").Success)
                {
                    name = Regex.Replace(name, "([0-9]+)", $"{depth + 1}");
                }
                else
                {
                    name = name + " (1)";
                }

                name = GetValidName(t, name, depth + 1);
            }

            return name;
        }
    }
}