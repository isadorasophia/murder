using ImGuiNET;
using Microsoft.Xna.Framework.Input;
using Murder.Assets;
using Murder.Core.Input;
using Murder.Diagnostics;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Services;
using Murder.Editor.Utilities;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Numerics;
using static Murder.Editor.ImGuiExtended.SearchBox;

namespace Murder.Editor;

public partial class EditorScene
{
    private static readonly Vector2 _commandPaletteWindowSize = new(400, 350);
    private static readonly Vector2 _commandPalettePadding = new(20, 0);
    private static readonly Vector2 _commandPaletteSearchBoxPadding = new (20, 20);
    private static readonly SearchBox.SearchBoxConfiguration _commandPaletteSizeConfiguration = new(
        SearchFrameSize: _commandPaletteWindowSize - _commandPalettePadding,
        SearchBoxContainerSize: _commandPaletteWindowSize - _commandPaletteSearchBoxPadding
    );
    
    private readonly ImmutableDictionary<ShortcutGroup, List<Shortcut>> _shortcuts;
    private readonly Dictionary<string, Shortcut> _shortcutSearchValues;

    private bool _commandPaletteIsVisible;
    private readonly Lazy<Dictionary<string, Shortcut>> _shortcutSearchValuesCache;

    private ImmutableDictionary<ShortcutGroup, List<Shortcut>> CreateShortcutList() =>
        new Dictionary<ShortcutGroup, List<Shortcut>>
        {
            [ShortcutGroup.Game] =
            [
                new ActionShortcut("Play Game", Keys.F5, StartGame)
            ],
            [ShortcutGroup.Edit] =
            [
                new ActionShortcut("Undo", new Chord(Keys.Z, InputHelpers.OSActionModifier), Architect.Undo.Undo),
                new ActionShortcut("Redo", new Chord(Keys.Y, InputHelpers.OSActionModifier), Architect.Undo.Redo),
            ],
            [ShortcutGroup.View] =
            [
                new ActionShortcut("Game Logger", Keys.F1, ToggleGameLogger),
                new ToggleShortcut("ImGui Demo", new Chord(Keys.G, InputHelpers.OSActionModifier, Keys.LeftShift),
                    ToggleImGuiDemo),
                new ToggleShortcut("Imgui Metrics", new Chord(Keys.M, InputHelpers.OSActionModifier, Keys.LeftShift),
                    ToggleShowingMetricsWindow),
                new ToggleShortcut("Style Editor", new Chord(Keys.E, InputHelpers.OSActionModifier, Keys.LeftShift),
                    ToggleShowingStyleEditor)
            ],
            [ShortcutGroup.Assets] =
            [
                new ActionShortcut("Save All Assets", Chord.None,
                    SaveAllAssets),
                new ActionShortcut("Bake Aseprite Guids", new Chord(Keys.B, InputHelpers.OSActionModifier, Keys.LeftShift),
                    BakeAsepriteGuids)
            ],
            [ShortcutGroup.Reload] =
            [
                new ActionShortcut("Shaders", Keys.F6, ReloadShaders),
                new ActionShortcut("Sounds", Keys.F7, ReloadSounds),
                new ActionShortcut("Metadata", new Chord(Keys.F8, Keys.LeftShift), ReloadMetadata),
                new ToggleShortcut(
                    name: "Always Build Atlas (on Startup)",
                    chord: new Chord(Keys.F3, Keys.LeftShift),
                    toggle: ReloadAtlasWithChangesToggled,
                    defaultCheckedValue: Architect.EditorSettings.AlwaysBuildAtlasOnStartup
                ),
                new ToggleShortcut(
                    name: "Hot Reload for Shaders",
                    chord: new Chord(Keys.F6, Keys.LeftShift),
                    toggle: ToggleHotReloadShader,
                    defaultCheckedValue: Architect.EditorSettings.AutomaticallyHotReloadShaderChanges
                ),
                new ToggleShortcut(
                    name: "Hot Reload for Dialogues",
                    chord: new Chord(Keys.F7, Keys.LeftShift),
                    toggle: ToggleHotReloadDialogue,
                    defaultCheckedValue: Architect.EditorSettings.EnableDialogueHotReload
                ),
                new ToggleShortcut(
                    name: "Hot Reload for Localization",
                    chord: new Chord(Keys.F8, Keys.LeftShift),
                    toggle: ToggleHotReloadLocalization,
                    defaultCheckedValue: Architect.EditorSettings.AutomaticallyHotReloadLocalizationChanges
                )
            ],
            [ShortcutGroup.Tools] =
            [
                new ActionShortcut("Refresh Window", Keys.F4, RefreshEditorWindow),
                new ActionShortcut("Run Diagnostics", new Chord(Keys.D, InputHelpers.OSActionModifier, Keys.LeftShift), RunDiagnostics),
                new ActionShortcut("Run All Diagnostics", new Chord(Keys.S, InputHelpers.OSActionModifier, Keys.LeftShift), RunAllDiagnostics),
                new ActionShortcut("Command Palette", new Chord(Keys.A, InputHelpers.OSActionModifier, Keys.LeftShift), ToggleCommandPalette)
            ],
            [ShortcutGroup.Publish] =
            [
                new ActionShortcut("Ready to ship!", new Chord(Keys.P, InputHelpers.OSActionModifier, Keys.Enter), PackAssetsToPublishedGame)
            ]
        }.ToImmutableDictionary();

    private void ToggleCommandPalette()
    {
        _commandPaletteIsVisible = !_commandPaletteIsVisible;
    }

    private void ToggleHotReloadLocalization(bool value)
    {
        Architect.EditorData.ToggleHotReloadLocalization(value);
    }

    private void ToggleHotReloadShader(bool value)
    {
        Architect.EditorData.ToggleHotReloadShader(value);
    }

    private void ToggleHotReloadDialogue(bool value)
    {
        Architect.EditorData.ToggleHotReloadDialogue(value);
    }

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

    private void UpdateShortcuts()
    {
        if (_changingScenesLock > 0)
        {
            _changingScenesLock--;
            return;
        }

        foreach (ShortcutGroup group in _shortcuts.Keys)
        {
            foreach (Shortcut shortcut in _shortcuts[group])
            {
                if (Game.Input.Shortcut(shortcut.Chord))
                {
                    shortcut.Execute();
                }
            }
        }
    }

    private void DrawMainMenuBar()
    {
        ImGui.BeginMainMenuBar();
        
        foreach (ShortcutGroup group in _shortcuts.Keys)
        {
            // Storing this value because the keyboard shortcuts must be verified regardless.
            bool menuShouldBeDrawn = ImGui.BeginMenu($"{group}");

            foreach (Shortcut shortcut in _shortcuts[group])
            {
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

        ImGui.EndMainMenuBar();

        // We need to check for the visibility of escapable elements in order and dismiss whatever is on top.
        if (Game.Input.Shortcut(Keys.Escape))
        {
            // First, we hide the command palette, since it takes the whole screen.
            if (_commandPaletteIsVisible)
            {
                _commandPaletteIsVisible = false;
            }
            // Next we hide the logger.
            else if (GameLogger.IsShowing)
            {
                ToggleGameLogger();
            }
        }
        
        if (Game.Input.Shortcut(Keys.W, InputHelpers.OSActionModifier) ||
            Game.Input.Shortcut(Keys.W, InputHelpers.OSActionModifier))
        {
            if (_selectedAssets.Count > 0)
            {
                CloseTab(_selectedAssets[_selectedTab]);
            }
        }

        if (Game.Input.Shortcut(Keys.F, InputHelpers.OSActionModifier) || 
            Game.Input.Shortcut(Keys.F, InputHelpers.OSActionModifier))
        {
            _focusOnFind = true;
        }

        if (_commandPaletteIsVisible)
        {    
            Vector2 viewportSize = ImGui.GetMainViewport().Size;

            // Background of the command palette, slightly darker and prevents interaction with the editor.
            ImGui.SetNextWindowBgAlpha(0.5f);
            ImGui.Begin("Command Palette Background", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize);
            ImGui.SetWindowPos(Vector2.Zero);
            ImGui.SetWindowSize(viewportSize);
            ImGui.End();
            
            // Command palette window.
            ImGui.Begin("Command Palette",  ImGuiWindowFlags.Modal | ImGuiWindowFlags.NoDecoration);
            ImGui.SetWindowPos((viewportSize - _commandPaletteWindowSize) / 2f);
            ImGui.SetWindowPos((viewportSize - _commandPaletteWindowSize) / 2f);
            ImGui.SetWindowSize(_commandPaletteWindowSize);
            ImGui.SetWindowFocus();

            SearchBoxSettings<Shortcut> settings = new(initialText: "Type a command");

            if (SearchBox.Search(
                $"command_palette", 
                settings,
                values: _shortcutSearchValuesCache,
                flags: SearchBoxFlags.Unfolded,
                sizeConfiguration: _commandPaletteSizeConfiguration, 
                out Shortcut? shortcut))
            {
                shortcut.Execute();
                _commandPaletteIsVisible = false;
            }
            
            ImGui.End();
        }
    }

    private static bool DrawShortcut(Shortcut shortcut)
    {
        if (shortcut is ToggleShortcut toggleShortcut)
        {
            bool pressed = ImGui.MenuItem(shortcut.Name, shortcut.Chord.ToString(), ref toggleShortcut.Checked);
            if (pressed)
            {
                toggleShortcut.Execute();
            }

            return pressed;
        }
        
        return ImGui.MenuItem(shortcut.Name, shortcut.Chord.ToString());
    }

    private void RefreshEditorWindow()
    {
        Architect.Instance.SaveWindowPosition();
        Architect.Instance.RefreshWindow();
    }

    private void PackAssetsToPublishedGame()
    {
        Architect.EditorData.PackPublishedGame();
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
            if (instance?.Editor.RunDiagnostics(asset.Guid) ?? true)
            {
                GameLogger.Log($"\uf00c Successfully ran diagnostics on {asset.Name}.");
            }
            else
            {
                GameLogger.Log($"\uf00d Issue found while running diagnostics on {asset.Name}.");
            }
        }
    }

    private void RunAllDiagnostics()
    {
        if (_selectedTab == Guid.Empty || !_selectedAssets.TryGetValue(_selectedTab, out GameAsset? asset))
        {
            GameLogger.Warning("An asset must be opened in order to run all diagnostics.");
        }
        else
        {
            var allAssetsOfType = Game.Data.FilterAllAssets(asset.GetType()).Select(g => g.Value).OrderBy(g => g.FilePath);
            foreach (GameAsset otherAsset in allAssetsOfType)
            {
                CustomEditorInstance? instance = GetOrCreateAssetEditor(otherAsset);
                if (instance?.Editor.RunDiagnostics(otherAsset.Guid) ?? true)
                {
                    // GameLogger.Log($"\uf00c Successfully ran diagnostics on {otherAsset.Name}.");
                }
                else
                {
                    GameLogger.Log($"\uf00d Issue found while running diagnostics on {otherAsset.Name}.");
                }
            }
        }
    }

    private void ReloadAtlasWithChangesToggled(bool value)
    {
        Architect.EditorSettings.AlwaysBuildAtlasOnStartup = value;

        // Persisted changes immediately.
        Architect.EditorData.SaveAsset(Architect.EditorSettings);
    }

    private static void ReloadMetadata()
    {
        AssetsFilter.RefreshCache();
        ReflectionHelper.ClearCache();
    }

    private static void ReloadShaders()
    {
        Architect.EditorData.ReloadShaders();
    }

    private static void ReloadSounds()
    {
        _ = Game.Data.LoadSoundsAsync(reload: true);
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
        Edit,
        Assets,
        View,
        Reload,
        Tools,
        Publish
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