using Bang.Components;
using Bang.Interactions;
using Bang.StateMachines;
using System.Collections.Immutable;
using System.Reflection;
using Murder.Assets;
using Murder.Core.Dialogs;
using Murder.Diagnostics;
using Murder.Attributes;
using System.Text.RegularExpressions;
using Murder.Utilities.Attributes;
using Murder.Core.Physics;

namespace Murder.Editor.Utilities
{
    internal static class AssetsFilter
    {
        private readonly static Lazy<ImmutableArray<(string name, int id)>> _collisionLayers = new(() =>
        {
            var layers = new List<(string name, int id)>();
            foreach (var type in ReflectionHelper.GetAllImplementationsOf<CollisionLayersBase>())
            {
                var contants = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
                foreach (var constant in contants)
                {
                    var item = (constant.Name, (int)constant.GetValue(null)!);
                    if (layers.Contains(item))
                        continue;

                    layers.Add(item);
                }
            }

            return layers.ToImmutableArray();
        });
        private readonly static Lazy<string[]> _collisionLayersNames = new(() =>
        {
            return CollisionLayers.Select(item => Prettify.CapitalizeFirstLetter(item.name)).ToArray();
        });

        public static ImmutableArray<(string name, int id)> CollisionLayers => _collisionLayers.Value;
        public static string[] CollisionLayersNames => _collisionLayersNames.Value;

        private static readonly Lazy<ImmutableArray<Type>> _componentTypes = new(() =>
        {
            return ReflectionHelper.GetAllImplementationsOf<IComponent>()
                .Where(t => !Attribute.IsDefined(t, typeof(HideInEditorAttribute))
                    && !typeof(IMessage).IsAssignableFrom(t)
                    && !Attribute.IsDefined(t, typeof(RuntimeOnlyAttribute)))
                .ToImmutableArray();
        });

        public static ImmutableArray<Type> GetAllComponents() => _componentTypes.Value;

        private static readonly Lazy<ImmutableArray<Type>> _stateMachines = new(() =>
        {
            return ReflectionHelper.GetAllImplementationsOf<StateMachine>()
                .Where(t => !Attribute.IsDefined(t, typeof(RuntimeOnlyAttribute)))
                .ToImmutableArray();
        });

        public static ImmutableArray<Type> GetAllStateMachines() => _stateMachines.Value;

        private static readonly Lazy<ImmutableArray<Type>> _iteractions = new(() =>
        {
            return ReflectionHelper.GetAllImplementationsOf<Interaction>()
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
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.GetInterfaces().Contains(typeof(Bang.Systems.ISystem)) && !type.IsInterface);
        }

        public static IEnumerable<Type> GetFromInterface(Type @interface)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.GetInterfaces().Contains(@interface) && !type.IsInterface);
        }

        private static ImmutableArray<Fact>? _blackboards = null;
        
        public static void RefreshCache() => _blackboards = null;

        public static ImmutableArray<Fact> GetAllFactsFromBlackboards() =>
            _blackboards ??= FetchAllFactsFromBlackboards();

        private static ImmutableArray<Fact> FetchAllFactsFromBlackboards()
        {
            IEnumerable<Type> blackboardTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => Attribute.IsDefined(t, typeof(BlackboardAttribute)));

            var facts = ImmutableArray.CreateBuilder<Fact>();
            foreach (Type t in blackboardTypes)
            {
                BlackboardAttribute blackboard = (BlackboardAttribute)Attribute.GetCustomAttribute(t, typeof(BlackboardAttribute))!;

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
                    else
                    {
                        GameLogger.Fail($"Unable to create a blackboard fact for variable {fieldType.Name}!");
                        continue;
                    }

                    Fact fact = new(blackboard.Name, field.Name, kind);
                    facts.Add(fact);
                }
            }

            return facts.ToImmutable();
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
