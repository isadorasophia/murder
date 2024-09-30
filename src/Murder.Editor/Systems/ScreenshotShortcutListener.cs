using Bang.Entities;
using Bang.Contexts;
using Bang.Systems;
using Murder.Core.Graphics;
using Murder.Editor.Components;
using Murder.Editor.Utilities;
using ImGuiNET;
using Murder.Serialization;
using Murder.Utilities;
using Murder.Services;

namespace Murder.Editor.Systems;

[Filter(ContextAccessorFilter.None)]
internal class ScreenshotShortcutListener : IMurderRenderSystem, IUpdateSystem, IGuiSystem
{
    private bool _menuTakeMultipleSelected = false;
    private bool _menuDelaySelected = false;

    private bool _takeScreenshot;

    private float _screenshotTaken = -10f;
    private float _delay = -1;
    private int _takeMultiple = 0;

    private readonly float _delayTime = 1f;
    public void Draw(RenderContext render, Context context)
    {
        if (render.TakingScreenshot)
        {
            return;
        }

        float showBorder = 1 - Calculator.ClampTime(_screenshotTaken, Game.NowUnscaled, 1);

        if (showBorder > 0f)
        {
            if (showBorder > 0.8f)
            {
                float flash = 1 - Calculator.ClampTime(_screenshotTaken, Game.NowUnscaled, 0.2f);

                RenderServices.DrawRectangle(render.DebugFxBatch, render.Camera.Bounds, Color.White * flash);
            }
            else
            {
                RenderServices.DrawRectangleOutline(render.DebugBatch, render.Camera.Bounds.Expand(-10), Color.White * showBorder, 1);
            }
        }

        if (_delay > 0)
        {
            float delayFade = 1- Calculator.ClampTime(_delay - Game.NowUnscaled, _delayTime);
            RenderServices.DrawRectangleOutline(render.DebugBatch, render.Camera.Bounds.Expand(-6 * delayFade), Color.Orange * delayFade, 2);
        }

        if (_takeScreenshot)
        {
            _takeScreenshot = false;
            _screenshotTaken = Game.NowUnscaled;
            _delay = -1;
            render.SaveGameplayScreenshot();

            if (_takeMultiple > 0)
            {
                _takeMultiple--;
                _delay = Game.NowUnscaled + _delayTime;
            }
        }

        if (Game.NowUnscaled > _delay && _delay>0)
        {
            _takeScreenshot = true;
        }
    }

    public void DrawGui(RenderContext render, Context context)
    {
        if (context.World.TryGetUnique<EditorComponent>() is not EditorComponent editorComponent)
        {
            return;
        }

        if (editorComponent.EditorHook.ShowDebug)
        {
            ImGui.BeginMainMenuBar();

            if (ImGui.BeginMenu("Screenshot"))
            {
                if (ImGui.MenuItem("Take screenshot", "F10"))
                {
                    StartScreenshot();
                }

                if (ImGui.MenuItem("Open screenshot folder", ""))
                {
                    FileHelper.OpenFolderOnOS(FileHelper.GetScreenshotFolder());
                }

                ImGui.MenuItem("Take multiple", "", ref _menuTakeMultipleSelected);
                ImGui.MenuItem("Delay", "", ref _menuDelaySelected);

                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();
        }
    }

    public void Update(Context context)
    {
        if (Game.Input.Shortcut(Microsoft.Xna.Framework.Input.Keys.F10))
        {
            Game.Input.ConsumeAll();
            StartScreenshot();
        }
    }

    private void StartScreenshot()
    {
        if (_delay < 0 && _takeMultiple == 0)
        {

            if (_menuDelaySelected)
            {
                _delay = Game.NowUnscaled + _delayTime * 2;
            }
            else
            {
                _takeScreenshot = true;
            }

            if (_menuTakeMultipleSelected)
            {
                _takeMultiple = 5;
            }
        }
    }
}