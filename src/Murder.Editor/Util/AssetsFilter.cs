using InstallWizard.Components;
using InstallWizard.Data;
using InstallWizard.Data.Dialogs;
using InstallWizard.Data.Prefabs;
using InstallWizard.DebugUtilities;
using InstallWizard.Dungeons;
using Editor.CustomFields;
using Bang.Components;
using Bang.Interactions;
using Bang.StateMachines;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using Murder.Assets;

namespace Editor.Util
{
    internal static class AssetsFilter
    {
        private static readonly Lazy<ImmutableArray<Type>> _componentTypes = new(() =>
        {
            return ReflectionHelper.GetAllImplementationsOf<IComponent>()
                .Where(t => !Attribute.IsDefined(t, typeof(HideInEditorAttribute)) && !typeof(IMessage).IsAssignableFrom(t))
                .ToImmutableArray();
        });

        public static ImmutableArray<Type> GetAllComponents() => _componentTypes.Value;

        private static readonly Lazy<ImmutableArray<Type>> _stateMachines = new(() =>
        {
            return ReflectionHelper.GetAllImplementationsOf<StateMachine>()
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

        public static ImmutableArray<Type> GetAllDungeons() => _dungeons.Value;

        private static readonly Lazy<ImmutableArray<Type>> _dungeons = new(() =>
        {
            return ReflectionHelper.GetAllImplementationsOf<DungeonGeneratorAsset>()
                .ToImmutableArray();
        });

        public static ImmutableArray<Fact> GetAllFactsFromBlackboards() => _blackboards.Value;

        private static readonly Lazy<ImmutableArray<Fact>> _blackboards = new(() =>
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
        });

    }
}
