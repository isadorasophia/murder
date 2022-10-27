using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using Murder.ImGuiExtended;
using Murder.Core;
using Murder.Core.Graphics;
using Murder.Core.Geometry;
using Murder.Utilities;
using Murder.Editor.Components;
using Murder.Editor.Systems;
using Murder.Systems.Graphics;
using Murder.Systems;
using Murder.Editor.Utilities;

namespace Murder.Editor.Stages
{
    /// <summary>
    /// Base implementation for rendering the world in the screen.
    /// </summary>
    public partial class Stage
    {
        /// <summary>
        /// Systems that are used for editor stages.
        /// </summary>
        private static readonly (Bang.Systems.ISystem, bool)[] _editorSystems = new (Bang.Systems.ISystem, bool)[]
        {
            (new EditorSystem(), true),
            (new MakeEverythingVisibleSystem(), true),
            (new EditorCameraControllerSystem(), true),
            (new EditorFloorRenderSystem(), true),
            (new TextureRenderSystem(), true),
            (new AsepriteRenderSystem_Simple(), true),
            (new AsepriteRenderDebugSystem(), true),
            (new DebugColliderRenderSystem(), true),
            (new CursorSystem(), true),
            (new TilemapRenderSystem(), true),
            (new TextBoxRenderSystem(), true),
            (new RectangleRenderSystem(), true),
            (new RectPositionDebugRenderer(), true),
            (new UpdatePositionSystem(), true),
            (new UpdateColliderSystem(), true),
            (new StateMachineSystem(), true),
            (new CustomDrawRenderSystem(), true)
        };

        protected readonly MonoWorld _world;

        private readonly RenderContext _renderContext;
        private readonly ImGuiRenderer _imGuiRenderer;

        /// <summary>
        /// Texture used by ImGui when printing in the screen.
        /// </summary>
        private IntPtr _imGuiRenderTexturePtr;

        public readonly EditorHook EditorHook;

        public Stage(ImGuiRenderer imGuiRenderer)
        {
            _imGuiRenderer = imGuiRenderer;
            _renderContext = new(Game.GraphicsDevice, new(320, 240, 2));

            _world = new MonoWorld(_editorSystems, _renderContext.Camera, Guid.Empty);
            _renderContext.RenderToScreen = false;

            var editorComponent = new EditorComponent();
            EditorHook = editorComponent.EditorHook;
            EditorHook.ShowDebug = true;

            _world.AddEntity(editorComponent);

            if (_renderContext.LastRenderTarget is RenderTarget2D target)
            {
                _imGuiRenderTexturePtr = _imGuiRenderer.BindTexture(target);
            }
        }

        public async ValueTask Draw()
        {
            ImGui.InvisibleButton("map_canvas", ImGui.GetContentRegionAvail());
            float DPIDownsize = Architect.Instance.DPIScale / 100f;
            var size = ImGui.GetItemRectSize() - new Vector2(0, 5).ToSys() * DPIDownsize;
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
            if (_renderContext.RefreshWindow(cameraSize, cameraScale))
            {
                _imGuiRenderTexturePtr = _imGuiRenderer.BindTexture(_renderContext.LastRenderTarget!);
                _renderContext.Camera.Position = -new Vector2(cameraSize.X / 2f, cameraSize.Y / 2f);
            }

            var topLeft = ImGui.GetItemRectMin();
            if (_world.GetUnique<EditorComponent>() is EditorComponent editorComponent)
            {
                editorComponent.EditorHook.Offset = ImGui.GetItemRectMin().ToPoint();
                Vector2 rectSize = ImGui.GetItemRectSize();
                editorComponent.EditorHook.StageSize = rectSize;
            }

            var bottomRight = ImGui.GetItemRectMax();

            ImDrawListPtr drawList = ImGui.GetWindowDrawList();
            drawList.PushClipRect(topLeft, bottomRight);

            await DrawWorld();

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

        private async ValueTask DrawWorld()
        {
            await _world.Update();
            
            _renderContext.Begin();
            await _world.Draw(_renderContext);
            _renderContext.End();
        }
    }
}
