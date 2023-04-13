using ImGuiNET;
using Murder.Editor.ImGuiExtended;
using Murder.Core;
using Murder.Core.Graphics;
using Murder.Core.Geometry;
using Murder.Utilities;
using Murder.Editor.Components;
using Murder.Editor.Utilities;
using Murder.Services;
using Microsoft.Xna.Framework.Graphics;

namespace Murder.Editor.Stages
{
    /// <summary>
    /// Base implementation for rendering the world in the screen.
    /// </summary>
    public partial class Stage : IDisposable
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

        public bool ShowInfo { get; set; } = true;

        public Stage(ImGuiRenderer imGuiRenderer, RenderContext renderContext, Guid? worldGuid = null)
        {
            _imGuiRenderer = imGuiRenderer;
            _renderContext = renderContext;
            
            _world = new MonoWorld(StageHelpers.FetchEditorSystems(), _renderContext.Camera, worldGuid ?? Guid.Empty);

            EditorComponent editorComponent = new();

            EditorHook = editorComponent.EditorHook;
            EditorHook.ShowDebug = true;
            EditorHook.GetEntityIdForGuid = GetEntityIdForGuid;
            EditorHook.GetNameForEntityId = GetNameForEntityId;
            EditorHook.EnableSelectChildren = worldGuid is null;

            _world.AddEntity(editorComponent);
        }

        private void InitializeDrawAndWorld()
        {
            _calledStart = true;

            if (_renderContext.LastRenderTarget is RenderTarget2D target)
            {
                _imGuiRenderTexturePtr = _imGuiRenderer.BindTexture(target);
            }

            _world.Start();
        }

        public void Draw()
        {
            if (!_calledStart)
            {
                InitializeDrawAndWorld();
            }

            ImGui.InvisibleButton("map_canvas", ImGui.GetContentRegionAvail() - new System.Numerics.Vector2(0, 5));

            System.Numerics.Vector2 size = ImGui.GetItemRectSize() - new Vector2(0, 5).ToSys();
            if (size.X <= 0 || size.Y <= 0)
            {
                // Empty.
                return;
            }

            float maxAxis = Math.Max(size.X, size.Y);
            Vector2 ratio = size.ToCore() / maxAxis;
            int maxSize = Calculator.RoundToInt(maxAxis);

            var cameraSize = new Point(Calculator.RoundToEven(ratio.X * maxSize), Calculator.RoundToEven(ratio.Y * maxSize));

            if (cameraSize != _renderContext.Camera.Size)
            {
                Point diff = _renderContext.Camera.Size - cameraSize;

                var dpi = ImGui.GetIO().FontGlobalScale;
                // TODO : Implement DPI
                if (_renderContext.RefreshWindow(cameraSize, 6))
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

            if (ShowInfo)
            {
                // Add useful coordinates
                var cursorWorld = EditorHook.CursorWorldPosition;
                var cursorScreen = EditorHook.CursorScreenPosition;
                
                DrawTextRoundedRect(drawList, new Vector2(10, 10).ToSys() + topLeft,
                    Game.Profile.Theme.Bg, Game.Profile.Theme.Accent,
                    $"Cursor: {cursorWorld.X}, {cursorWorld.Y}");

                if (!EditorHook.SelectionBox.IsEmpty)
                {
                    DrawTextRoundedRect(drawList, new Vector2(10, 30).ToSys() + topLeft,
                        Game.Profile.Theme.Bg, Game.Profile.Theme.Accent,
                        $"Rect: {EditorHook.SelectionBox.X:0.##}, {EditorHook.SelectionBox.Y:0.##}, {EditorHook.SelectionBox.Width:0.##}, {EditorHook.SelectionBox.Height:0.##}");
                }
            }

            drawList.PopClipRect();

            Architect.EditorSettings.CameraPositions[_world.Guid()] = _renderContext.Camera.Position.Point;
        }

        private static void DrawTextRoundedRect(ImDrawListPtr drawList, System.Numerics.Vector2 position, System.Numerics.Vector4 bgColor, System.Numerics.Vector4 textColor, string text)
        {
            drawList.AddRectFilled(position+ new System.Numerics.Vector2(-4,-2), position + new System.Numerics.Vector2(text.Length*7+ 4, 16),
                ImGuiHelpers.MakeColor32(bgColor), 8f);
            drawList.AddText(position, ImGuiHelpers.MakeColor32(textColor), text);
        }

        private float _targetFixedUpdateTime = 0;

        private void DrawWorld()
        {
            _world.Update();

            if (Game.NowUnescaled >= _targetFixedUpdateTime)
            {
                _world.FixedUpdate();
                _targetFixedUpdateTime = Game.NowUnescaled + Game.FixedDeltaTime;
            }

            _renderContext.Begin();
            _world.Draw(_renderContext);
            _renderContext.End();
        }

        public void Dispose()
        {
            _renderContext?.Dispose();
        }

        internal void ResetCamera()
        {
            EditorHook.CurrentZoomLevel = EditorHook.STARTING_ZOOM;
            _renderContext.Camera.Zoom = 1;
            _renderContext.Camera.Position = Vector2.Zero;
        }
    }
}
