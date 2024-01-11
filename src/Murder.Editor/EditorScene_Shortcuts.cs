using ImGuiNET;
using Microsoft.Xna.Framework.Input;
using Murder.Assets;
using Murder.Core.Input;
using Murder.Diagnostics;
using Murder.Editor.Services;
using Murder.Editor.Utilities;
using System.Collections.Immutable;

namespace Murder.Editor;

public partial class EditorScene
{
    private readonly ImmutableDictionary<ShortcutGroup, List<Shortcut>> _shortcuts;

    private ImmutableDictionary<ShortcutGroup, List<Shortcut>> CreateShortcutList() =>
        new Dictionary<ShortcutGroup, List<Shortcut>>
        {
            [ShortcutGroup.Game] =
            [
                new ActionShortcut("Play Game", Keys.F5, StartGame)
            ],
            [ShortcutGroup.View] =
            [
                new ActionShortcut("Game Logger", Keys.F1, ToggleGameLogger),
                new ToggleShortcut("ImGui Demo", new Chord(Keys.G, _leftOsActionModifier, Keys.LeftShift),
                    ToggleImGuiDemo),
                new ToggleShortcut("Metrics", new Chord(Keys.M, _leftOsActionModifier, Keys.LeftShift),
                    ToggleShowingMetricsWindow),
                new ToggleShortcut("Style Editor", new Chord(Keys.E, _leftOsActionModifier, Keys.LeftShift),
                    ToggleShowingStyleEditor)
            ],
            [ShortcutGroup.Assets] =
            [
                new ActionShortcut("Save All Assets", new Chord(Keys.S, _leftOsActionModifier, Keys.LeftShift),
                    SaveAllAssets),
                new ActionShortcut("Bake Aseprite Guids", new Chord(Keys.B, _leftOsActionModifier, Keys.LeftShift),
                    BakeAsepriteGuids)
            ],
            [ShortcutGroup.Reload] = 
            [
                new ActionShortcut("Content and Atlas", Keys.F3, ReloadContentAndAtlas),
                new ActionShortcut("Shaders", Keys.F6, ReloadShaders),
                new ActionShortcut("Sounds", Keys.F7, ReloadSounds),
                new ToggleShortcut(
                    name: "Only Reload Atlas With Changes",
                    chord: new Chord(Keys.F3, Keys.LeftShift),
                    toggle: ReloadAtlasWithChangesToggled,
                    defaultCheckedValue: Architect.EditorSettings.OnlyReloadAtlasWithChanges
                )
            ],
            [ShortcutGroup.Tools] =
            [
                new ActionShortcut("Refresh Window", Keys.F4, RefreshEditorWindow),
                new ActionShortcut("Run diagnostics", new Chord(Keys.D, _leftOsActionModifier, Keys.LeftShift),  RunDiagnostics)
            ]
        }.ToImmutableDictionary();

    private void ToggleShowingStyleEditor(bool value)
    {
        _showStyleEditor = value;
    }

    private void ToggleShowingMetricsWindow(bool value)
    {
        _showingMetricsWindow = value;
    }

    private void ToggleImGuiDemo(bool value)
    {
        _showingImguiDemoWindow = value;
    }
    
    private void DrawMainMenuBar()
    {
        ImGui.BeginMainMenuBar();
        foreach (ShortcutGroup group in _shortcuts.Keys)
        {
            // Storing this value because the keyboard shortcuts must be verified regardless.
            var menuShouldBeDrawn = ImGui.BeginMenu(group.ToString());

            foreach (Shortcut shortcut in _shortcuts[group])
            {
                if (_changingScenesLock < 0)
                {
                    if (Game.Input.Shortcut(shortcut.Chord))
                    {
                        shortcut.Execute();
                    }
                }

                if (menuShouldBeDrawn && DrawShortcut(shortcut))
                {
                    shortcut.Execute();
                }
            }

            // We only need to end a menu that was rendered.
            if (menuShouldBeDrawn)
            {
                ImGui.EndMenu();
            }
        }

        _changingScenesLock--;
        ImGui.EndMainMenuBar();
        
        if (Game.Input.Shortcut(Keys.Escape) && GameLogger.IsShowing)
        {
            ToggleGameLogger();
        }
        
        if (Game.Input.Shortcut(Keys.W, _leftOsActionModifier) ||
            Game.Input.Shortcut(Keys.W, _rightOsActionModifier))
        {
            CloseTab(_selectedAssets[_selectedTab]);
        }

        if (Game.Input.Shortcut(Keys.F, _leftOsActionModifier) || 
            Game.Input.Shortcut(Keys.F, _rightOsActionModifier))
        {
            _focusOnFind = true;
        }
    }

    private static bool DrawShortcut(Shortcut shortcut)
    {
        if (shortcut is ToggleShortcut toggleShortcut)
        {
            return ImGui.MenuItem(shortcut.Name, shortcut.Chord.ToString(), ref toggleShortcut.Checked);
        }
        
        return ImGui.MenuItem(shortcut.Name, shortcut.Chord.ToString());
    }

    private void RefreshEditorWindow()
    {
        Architect.Instance.SaveWindowPosition();
        Architect.Instance.RefreshWindow();
    }

    private void RunDiagnostics()
    {
        if (_selectedTab == Guid.Empty || !_selectedAssets.TryGetValue(_selectedTab, out GameAsset? asset))
        {
            GameLogger.Warning("An asset must be opened in order to run diagnostics.");
        }
        else
        {
            CustomEditorInstance? instance = GetOrCreateAssetEditor(asset);
            if (instance?.Editor.RunDiagnostics() ?? true)
            {
                GameLogger.Log($"\uf00c Successfully ran diagnostics on {asset.Name}.");
            }
            else
            {
                GameLogger.Log($"\uf00d Issue found while running diagnostics on {asset.Name}.");
            }
        }
    }

    private void ReloadAtlasWithChangesToggled(bool value)
    {
        Architect.EditorSettings.OnlyReloadAtlasWithChanges = value;
    }

    private static void ReloadContentAndAtlas()
    {
        _ = Architect.EditorData.ReloadSprites();
        AssetsFilter.RefreshCache();
    }

    private static void ReloadShaders()
    {
        Architect.Instance.ReloadShaders();
    }

    private static void ReloadSounds()
    {
        _ = Game.Data.LoadSounds(reload: true);
    }

    private static void SaveAllAssets()
    {
        Architect.EditorData.SaveAllAssets();
    }

    private static void BakeAsepriteGuids()
    {
        AsepriteServices.BakeAllAsepriteFileGuid();
    }

    private void StartGame()
    {
        SaveEditorState();
        Architect.Instance.QueueStartPlayingGame(false);
    }

    private static void ToggleGameLogger()
    {
        GameLogger.GetOrCreateInstance().ToggleDebugWindow();
    }

    private enum ShortcutGroup
    {
        Game,
        Assets,
        View,
        Reload,
        Tools
    }

    private abstract record Shortcut(string Name, Chord Chord)
    {
        public abstract void Execute();
    }

    private sealed record ActionShortcut(
        string Name,
        Chord Chord,
        Action Action
    ) : Shortcut(Name, Chord)
    {
        public override void Execute()
        {
            Action();
        }
    }
    
    private sealed record ToggleShortcut(
        string Name,
        Chord Chord,
        Action<bool> Toggle
    ) : Shortcut(Name, Chord)
    {
        public bool Checked;

        public ToggleShortcut(
            string name,
            Chord chord,
            Action<bool> toggle,
            bool defaultCheckedValue
        ) : this(name, chord, toggle)
        {
            Checked = defaultCheckedValue;
        }

        public override void Execute()
        {
            Checked = !Checked;
            Toggle(Checked);
        }
    }
}