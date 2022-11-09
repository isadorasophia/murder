using Bang.Systems;
using Murder.Diagnostics;

namespace Murder.Editor.Utilities
{
    internal static class StageHelpers
    {
        public static IList<(ISystem system, bool isActive)> FetchEditorSystems()
        {
            List<(ISystem, bool)> systems = new();

            foreach ((Type t, bool isActive) in Architect.EditorSettings.EditorSystems)
            {
                if (Activator.CreateInstance(t) is ISystem system)
                {
                    systems.Add((system, isActive));
                }
                else
                {
                    GameLogger.Error($"The {t} is not a valid system!");
                }
            }

            return systems;
        }
    }
}
