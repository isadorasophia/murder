using Bang.Contexts;
using Bang.Systems;
using Microsoft.Xna.Framework.Input;
using Murder.Core.Geometry;
using Murder.Diagnostics;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Utilities;
using Murder.Systems;
using System.Collections.Immutable;

namespace Murder.Editor.Systems
{
    [DoNotPause]
    [Filter(ContextAccessorFilter.None)]
    public class DebugActivatorSystem : IUpdateSystem, IStartupSystem
    {
        private bool _showConsole = false;
        private bool _showEditorSystems = false;

        private IEnumerable<Type> _debugSystems = ImmutableArray<Type>.Empty;

        public void Start(Context context)
        {
            if (context.World.TryGetUnique<EditorComponent>() is EditorComponent editorComponent)
            {
                _showEditorSystems = editorComponent.EditorHook.ShowDebug;
            }

            _debugSystems = ReflectionHelper.GetAllTypesWithAttributeDefined<OnlyShowOnDebugViewAttribute>();

            UpdateConsoleSystem(context);

            // [Perf] Only deactivate editor systems. All remaining editor systems should be DEACTIVATED by default. Or else.
            context.World.DeactivateSystem<EditorSystem>();
        }

        public void Update(Context context)
        {
            var editorHook = context.World.GetUnique<EditorComponent>().EditorHook;

            if (Game.Input.Shortcut(Keys.F1) ||
                (_showConsole && Game.Input.Shortcut(Keys.Escape)))
            {
                _showConsole = !_showConsole;
                UpdateConsoleSystem(context);
            }

            if (GameLogger.IsShowing && !_showConsole)
            {
                _showConsole = true;
                UpdateConsoleSystem(context);
            }

            if (Game.Input.Shortcut(Keys.F2))
            {
                _showEditorSystems = !_showEditorSystems;
                editorHook.ShowDebug = _showEditorSystems;
                editorHook.StageSize = new Point(
                    Game.GraphicsDevice.Viewport.Width,
                    Game.GraphicsDevice.Viewport.Height);

                UpdateEditorSystems(context);
            }
        }

        private void UpdateConsoleSystem(Context context)
        {
            if (_showConsole)
            {
                context.World.ActivateSystem<ConsoleSystem>();
            }
            else
            {
                context.World.DeactivateSystem<ConsoleSystem>();
            }

            GameLogger.Toggle(_showConsole);
            UpdateEditorInput();
        }

        private void UpdateEditorSystems(Context context)
        {
            if (_showEditorSystems)
            {
                foreach (Type s in _debugSystems)
                {
                    context.World.ActivateSystem(s);
                }
            }
            else
            {
                foreach (Type s in _debugSystems)
                {
                    context.World.DeactivateSystem(s);
                }
            }

            UpdateEditorInput();
        }

        private void UpdateEditorInput()
        {
            if (_showConsole || _showEditorSystems)
            {
                TextInputEXT.StartTextInput();
            }
            else
            {
                TextInputEXT.StopTextInput();
            }
        }
    }
}