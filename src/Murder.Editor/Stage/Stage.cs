using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using Murder.Editor.ImGuiExtended;
using Murder.Core;
using Murder.Core.Graphics;
using Murder.Core.Geometry;
using Murder.Utilities;
using Murder.Editor.Components;
using Murder.Editor.Utilities;

namespace Murder.Editor.Stages
{
    /// <summary>
    /// Base implementation for rendering the world in the screen.
    /// </summary>
    public partial class Stage
    {
        protected readonly MonoWorld _world;

        private readonly RenderContext _renderContext;
        private readonly ImGuiRenderer _imGuiRenderer;

        private bool _calledStart = false;

        /// <summary>
        /// Texture used by ImGui when printing in the screen.
        /// </summary>
        private IntPtr _imGuiRenderTexturePtr;

        public readonly EditorHook EditorHook;

        public Stage(ImGuiRenderer imGuiRenderer, Guid? worldGuid = null)
        {
            _imGuiRenderer = imGuiRenderer;
            _renderContext = new(Game.GraphicsDevice, new(320, 240, 2), useCustomShader: false);

            _world = new MonoWorld(StageHelpers.FetchEditorSystems(), _renderContext.Camera, worldGuid ?? Guid.Empty);
            _renderContext.RenderToScreen = false;

            EditorComponent editorComponent = new();
            
            EditorHook = editorComponent.EditorHook;
            EditorHook.ShowDebug = true;
            EditorHook.GetEntityIdForGuid = GetEntityIdForGuid;
            EditorHook.GetNameForEntityId = GetNameForEntityId;

            _world.AddEntity(editorComponent);

            if (_renderContext.LastRenderTarget is RenderTarget2D target)
            {
                _imGuiRenderTexturePtr = _imGuiRenderer.BindTexture(target);
            }
        }

        public void Draw()
        {
            ImGui.InvisibleButton("map_canvas", ImGui.GetContentRegionAvail() - new System.Numerics.Vector2(0, 5.WithDpi()));

            float DPIDownsize = Architect.Instance.DPIScale / 100f;
            System.Numerics.Vector2 size = ImGui.GetItemRectSize() - new Vector2(0, 5).ToSys().WithDpi();
            if (size.X <= 0 || size.Y <= 0)
            {
                // Empty.
                return;
            }

            int cameraScale = Calculator.RoundToInt(DPIDownsize);
            float maxAxis = Math.Max(size.X, size.Y);
            Vector2 ratio = size.ToCore() / maxAxis;
            int maxSize = Calculator.RoundToInt(maxAxis / cameraScale);

            var cameraSize = new Point(Calculator.RoundToEven(ratio.X * maxSize), Calculator.RoundToEven(ratio.Y * maxSize));

            if (cameraSize != _renderContext.Camera.Size)
            {
                Point diff = _renderContext.Camera.Size - cameraSize;

                if (_renderContext.RefreshWindow(cameraSize, cameraScale))
                {
                    _imGuiRenderTexturePtr = _imGuiRenderer.BindTexture(_renderContext.LastRenderTarget!);
                    _renderContext.Camera.Position += diff / 2;
                }
            }

            var topLeft = ImGui.GetItemRectMin();
            if (_world.GetUnique<EditorComponent>() is EditorComponent editorComponent)
            {
                editorComponent.EditorHook.Offset = ImGui.GetItemRectMin().ToPoint();
                Vector2 rectSize = ImGui.GetItemRectSize();
                editorComponent.EditorHook.StageSize = rectSize;
            }

            System.Numerics.Vector2 bottomRight = ImGui.GetItemRectMax();

            ImDrawListPtr drawList = ImGui.GetWindowDrawList();
            drawList.PushClipRect(topLeft, bottomRight);

            DrawWorld();

            _imGuiRenderer.BindTexture(_imGuiRenderTexturePtr, _renderContext.LastRenderTarget!, unloadPrevious: false);
            drawList.AddImage(_imGuiRenderTexturePtr, topLeft, bottomRight);

            // Add useful coordinates
            drawList.AddText(new Vector2(10, 10).ToSys() + topLeft, ImGuiHelpers.MakeColor32(0, 0, 0, 255),
                $"Canvas Size: {size.X}, {size.Y} (Real:{cameraSize.X},{cameraSize.Y})");

            var cursorWorld = EditorHook.CursorWorldPosition;
            var cursorScreen = EditorHook.CursorScreenPosition;
            drawList.AddText(new Vector2(10, 50).ToSys() + topLeft, ImGuiHelpers.MakeColor32(0, 0, 0, 255),
                $"Cursor: (World {cursorWorld.X}, {cursorWorld.Y}) (Screen {cursorScreen.X}, {cursorScreen.Y})");

            drawList.AddText(new Vector2(10, 80).ToSys() + topLeft, ImGuiHelpers.MakeColor32(0, 0, 0, 255),
                $"Zoom: {_renderContext.Camera.Zoom}");

            drawList.PopClipRect();
        }

        private float _timeSinceFixedUpdate = 0;

        private void DrawWorld()
        {
            if (!_calledStart)
            {
                _calledStart = true;
                _world.Start();
            }

            _world.Update();

            if (Game.Now + Game.FixedDeltaTime >= _timeSinceFixedUpdate)
            {
                _world.FixedUpdate();

                _timeSinceFixedUpdate = Game.Now;
            }

            _renderContext.Begin();
            _world.Draw(_renderContext);
            _renderContext.End();
        }
    }
}
