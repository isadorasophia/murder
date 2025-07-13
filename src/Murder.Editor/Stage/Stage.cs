using Bang;
using Bang.Entities;
using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Editor.Assets;
using Murder.Editor.Components;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Utilities;
using Murder.Services;
using Murder.Utilities;
using System.Numerics;

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
        public string? FocusOnGroup = null;

        private readonly HashSet<int> _hiddenIds = [];

        [Flags]
        public enum StageType
        {
            None = 0,
            EnableSelectChildren = 1,
            PlayMode = 1
        }

        public Stage(ImGuiRenderer imGuiRenderer, RenderContext renderContext, StageType type, Guid? guid = null) :
            this(imGuiRenderer, renderContext, hook: new(type.HasFlag(StageType.PlayMode)), type, guid) { }

        public Stage(ImGuiRenderer imGuiRenderer, RenderContext renderContext, EditorHook hook, StageType type, Guid? guid = null)
        {
            _imGuiRenderer = imGuiRenderer;
            _renderContext = renderContext;

            _world = new MonoWorld(StageHelpers.FetchEditorSystems(), _renderContext.Camera, guid ?? Guid.Empty);

            EditorComponent editorComponent = new(hook);

            EditorHook = editorComponent.EditorHook;

            EditorHook.ShowDebug = true;
            EditorHook.GetEntityIdForGuid = GetEntityIdForGuid;
            EditorHook.GetNameForEntityId = GetNameForEntityId;
            EditorHook.EnableSelectChildren = type.HasFlag(StageType.EnableSelectChildren);

            if (guid is not null &&
                Architect.EditorSettings.StageInfo.TryGetValue(guid.Value, out PersistStageInfo info))
            {
                EditorHook.CurrentZoomLevel = info.Zoom;
                EditorHook.StageSettings = info.Settings;
            }

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

        public void Update()
        {
            if (!_calledStart)
            {
                InitializeDrawAndWorld();
            }

            // Only update the stage if it's active.
            if (Game.Instance.IsActive)
            {
                _world.Update();
            }

            if (Game.NowUnscaled >= _targetFixedUpdateTime)
            {
                _world.FixedUpdate();
                _targetFixedUpdateTime = Game.NowUnscaled + Game.FixedDeltaTime;
            }
        }

        public void Draw(Rectangle? rectToDrawStage = null)
        {
            if (!_calledStart)
            {
                InitializeDrawAndWorld();
            }

            ImGui.InvisibleButton("map_canvas", ImGui.GetContentRegionAvail() - new Vector2(0, 5));
            if (ImGui.IsItemHovered())
            {
                EditorHook.UsingGui = false;
            }
            else
            {
                EditorHook.UsingGui = true;
            }

            Vector2 topLeft = rectToDrawStage?.TopLeft ?? ImGui.GetItemRectMin();
            Vector2 bottomRight = rectToDrawStage?.BottomRight ?? ImGui.GetItemRectMax();

            Vector2 size = rectToDrawStage?.Size ?? Rectangle.FromCoordinates(topLeft, bottomRight).Size;

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

                if (_renderContext.RefreshWindow(Architect.GraphicsDevice, cameraSize, cameraSize, new ViewportResizeStyle(ViewportResizeMode.None)))
                {
                    if (_imGuiRenderTexturePtr == 0) // Not initialized yet
                    {
                        _imGuiRenderTexturePtr = _imGuiRenderer.BindTexture(_renderContext.LastRenderTarget!);
                    }
                    else // Just resizing the screen
                    {
                        _imGuiRenderer.BindTexture(_imGuiRenderTexturePtr, _renderContext.LastRenderTarget!, false);
                    }
                    _renderContext.Camera.Position += diff / 2;
                }
            }

            if (_world.GetUnique<EditorComponent>() is EditorComponent editorComponent)
            {
                editorComponent.EditorHook.Offset = (topLeft * Architect.EditorSettings.DpiScale).Point();
                editorComponent.EditorHook.StageSize = (rectToDrawStage?.Size ?? 
                    Rectangle.FromCoordinates(topLeft, bottomRight).Size) * Architect.EditorSettings.DpiScale;
            }

            ImDrawListPtr drawList = ImGui.GetWindowDrawList();
            drawList.PushClipRect(topLeft, bottomRight);

            DrawWorld();

            _imGuiRenderer.BindTexture(_imGuiRenderTexturePtr, _renderContext.LastRenderTarget!, unloadPrevious: false);
            // Draw the game window
            drawList.AddImage(_imGuiRenderTexturePtr, topLeft, bottomRight);

            if (EditorHook.EditorMode == EditorHook.EditorModes.EditMode)
            {
                uint faded = ImGuiHelpers.MakeColor32(new Vector4(Game.Profile.Theme.Accent.X, Game.Profile.Theme.Accent.Y, Game.Profile.Theme.Accent.Z, 0.30f));

                Point border = new Point(24, 24);
                drawList.AddRectFilled(topLeft, new Vector2(bottomRight.X, topLeft.Y + border.Y), faded, 0);
                drawList.AddRectFilled(topLeft + new Vector2(0, border.Y), new Vector2(topLeft.X + border.X, bottomRight.Y - border.Y), faded, 0);
                drawList.AddRectFilled(new Vector2(bottomRight.X - border.X, topLeft.Y + border.Y), bottomRight + new Vector2(0, -border.Y), faded, 0);
                drawList.AddRectFilled(new Vector2(topLeft.X, bottomRight.Y - border.Y), new Vector2(bottomRight.X, bottomRight.Y), faded, 0);
            }

            drawList.PopClipRect();

            Vector2 infoSize = Vector2.Zero;

            if (ShowInfo)
            {
                ImGui.SetNextWindowPos(topLeft);
                ImGui.SetNextWindowBgAlpha(0.85f);
                if (ImGui.Begin("Stage Info", 
                    ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse |
                    ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoNav |
                    ImGuiWindowFlags.NoDecoration))
                {
                    if (EditorHook.EditorMode == EditorHook.EditorModes.EditMode)
                    {
                        if (EditorHook.CanSwitchModes)
                        {
                            if (ImGui.Button("Edit Mode"))
                            {
                                EditorHook.EditorMode = EditorHook.EditorModes.ObjectMode;
                            }

                            ImGui.SameLine();
                            bool isActive = EditorHook.StageSettings.HasFlag(StageSetting.ShowSprite);
                            if (ImGuiHelpers.ColoredIconButton('\uf03e', "##modify_sprite_editor_enabled", isActive))
                            {
                                if (isActive)
                                {
                                    EditorHook.StageSettings &= ~StageSetting.ShowSprite;
                                    isActive = false;
                                }
                                else
                                {
                                    EditorHook.StageSettings |= StageSetting.ShowSprite;
                                    isActive = true;
                                }
                            }

                            ImGui.SameLine();
                            isActive = EditorHook.StageSettings.HasFlag(StageSetting.ShowCollider);
                            if (ImGuiHelpers.ColoredIconButton('\uf0c8', "##modify_collider_editor_enabled", isActive))
                            {
                                if (isActive)
                                {
                                    EditorHook.StageSettings &= ~StageSetting.ShowCollider;
                                    isActive = false;
                                }
                                else
                                {
                                    EditorHook.StageSettings |= StageSetting.ShowCollider;
                                    isActive = true;
                                }
                            }

                            if (Game.Input.Shortcut(Microsoft.Xna.Framework.Input.Keys.D1))
                            {
                                EditorHook.StageSettings |= StageSetting.ShowSprite;
                                EditorHook.StageSettings &= ~StageSetting.ShowCollider;
                            }
                            if (Game.Input.Shortcut(Microsoft.Xna.Framework.Input.Keys.D2))
                            {
                                EditorHook.StageSettings |= StageSetting.ShowCollider;
                                EditorHook.StageSettings &= ~StageSetting.ShowSprite;
                            }

                            ImGui.TextColored(Game.Profile.Theme.Faded, "[Press TAB to exit]");
                        }
                        else
                        {
                            ImGui.TextColored(Game.Profile.Theme.HighAccent, "Edit Mode");
                            ImGui.Separator();

                            bool isActive = EditorHook.StageSettings.HasFlag(StageSetting.ShowSound);
                            if (ImGuiHelpers.ColoredIconButton('\uf028', "##modify_sound_editor_enabled", isActive))
                            {
                                if (isActive)
                                {
                                    EditorHook.StageSettings &= ~StageSetting.ShowSound;
                                    isActive = false;
                                }
                                else
                                {
                                    EditorHook.StageSettings |= StageSetting.ShowSound;
                                    isActive = true;
                                }
                            }
                            ImGuiHelpers.HelpTooltip(isActive ? "Hide Sound Editor" : "Show Sound Editor");

                            ImGui.SameLine();
                            isActive = EditorHook.StageSettings.HasFlag(StageSetting.ShowSprite);
                            if (ImGuiHelpers.ColoredIconButton('\uf03e', "##modify_sprite_editor_enabled", isActive))
                            {
                                if (isActive)
                                {
                                    EditorHook.StageSettings &= ~StageSetting.ShowSprite;
                                    isActive = false;
                                }
                                else
                                {
                                    EditorHook.StageSettings |= StageSetting.ShowSprite;
                                    isActive = true;
                                }
                            }
                            ImGuiHelpers.HelpTooltip(isActive ? "Hide YSort Editor" : "Show YSort Editor");

                            ImGui.SameLine();
                            isActive = EditorHook.StageSettings.HasFlag(StageSetting.ShowCollider);
                            if (ImGuiHelpers.ColoredIconButton('\uf0c8', "##modify_collider_editor_enabled", isActive))
                            {
                                if (isActive)
                                {
                                    EditorHook.StageSettings &= ~StageSetting.ShowCollider;
                                    isActive = false;
                                }
                                else
                                {
                                    EditorHook.StageSettings |= StageSetting.ShowCollider;
                                    isActive = true;
                                }
                            }

                            ImGuiHelpers.HelpTooltip(isActive ? "Hide Collider Editor" : "Show Collider Editor");
                        }
                        ImGui.Separator();
                        foreach (var entity in EditorHook.AllSelectedEntities)
                        {
                            DrawCheckboxes(_world, entity.Key, GetNameForEntityId(entity.Key) ?? "Unnamed");
                        }
                        EditorHook.HideEditIds = _hiddenIds;
                    }
                    else if (EditorHook.EditorMode == EditorHook.EditorModes.ObjectMode)
                    {
                        if (EditorHook.AllSelectedEntities.Count > 0)
                        {
                            if (ImGui.Button("Object Mode"))
                            {
                                EditorHook.EditorMode = EditorHook.EditorModes.EditMode;
                            }

                            if (EditorHook.AllSelectedEntities.Count == 1 &&
                                EditorHook.AllSelectedEntities.First().Value is Entity selected)
                            {
                                if (selected.HasCollider() && selected.HasTransform())
                                {
                                    var boxSize = selected.GetColliderBoundingBox();
                                    ImGui.TextColored(Game.Profile.Theme.Faded, $"{boxSize.Width}x{boxSize.Height}px");
                                }
                                else if (selected.TryGetSprite() is SpriteComponent sprite && Game.Data.TryGetAsset<SpriteAsset>(sprite.AnimationGuid) is SpriteAsset spriteAsset)
                                {
                                    ImGui.TextColored(Game.Profile.Theme.Faded, $"{spriteAsset.Size.X}x{spriteAsset.Size.Y}px");
                                }
                            }

                            ImGui.TextColored(Game.Profile.Theme.Faded, $"{EditorHook.AllSelectedEntities.Count} selected");
                        }
                        else
                        {
                            ImGui.TextColored(Game.Profile.Theme.HighAccent, "Object Mode");
                            ImGui.TextColored(Game.Profile.Theme.Faded, $"Click on an entity to select");
                        }

                    }
                    else
                    {
                        ImGui.TextColored(Game.Profile.Theme.HighAccent, "Play Mode");
                    }
                    infoSize = ImGui.GetWindowSize();
                    ImGui.End();
                }

                ImGui.SetNextWindowPos(new Vector2(topLeft.X + infoSize.X, topLeft.Y));
                ImGui.SetNextWindowBgAlpha(0.75f);
                if (ImGui.Begin("Cursor Info",
                    ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse |
                    ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoNav |
                    ImGuiWindowFlags.NoDecoration))
                {
                    if (!EditorHook.SelectionBox.IsEmpty)
                    {
                        ImGui.TextColored(Game.Profile.Theme.White, $"({EditorHook.SelectionBox.X}, {EditorHook.SelectionBox.Y}) ({EditorHook.SelectionBox.Width}, {EditorHook.SelectionBox.Height})");
                    }
                    else
                    {
                        ImGui.TextColored(Game.Profile.Theme.Faded, $"{EditorHook.CursorWorldPosition}");
                        // ImGui.TextColored(Game.Profile.Theme.Faded, $"{EditorHook.CursorScreenPosition}"); // use this to debug the screen position (useful for dpi)
                    }
                    ImGui.End();
                }
            }
        }
        public void PersistInfo(Guid guid)
        {
            // Persisted the last position.
            Architect.EditorSettings.StageInfo[guid] = new(
                _renderContext.Camera.Position.Point(),
                _renderContext.Camera.Size,
                EditorHook.CurrentZoomLevel,
                EditorHook.StageSettings);
        }

        private void DrawCheckboxes(World world, int id, string name)
        {
            if (world.TryGetEntity(id) is Entity entity)
            {
                bool isActive = !_hiddenIds.Contains(id);
                if (ImGui.Checkbox($"{name}({id})", ref isActive))
                {
                    if (!isActive)
                    {
                        _hiddenIds.Add(id);
                    }
                    else
                    {
                        _hiddenIds.Remove(id);
                    }
                }

                if(entity.TryGetCollider() is ColliderComponent collider)
                {
                    ImGui.SameLine();
                    ImGui.TextColored(collider.DebugColor.ToSysVector4(), $"[{collider.Shapes.Length} shapes]");
                }

                if (entity.TryGetSprite() is SpriteComponent sprite)
                {
                    ImGui.SameLine();
                    ImGui.TextColored(Game.Profile.Theme.Accent, $"[{Game.Data.TryGetAsset<SpriteAsset>(sprite.AnimationGuid)?.Name}]");
                }

                if (entity.TryGetAgentSprite() is AgentSpriteComponent agentSprite)
                {
                    ImGui.SameLine();
                    ImGui.TextColored(Game.Profile.Theme.Accent, $"[{Game.Data.TryGetAsset<SpriteAsset>(agentSprite.AnimationGuid)?.Name}]");
                }
                ImGui.TreePush($"{id}");
                foreach (var child in entity.FetchChildrenWithNames)
                {
                    DrawCheckboxes(world, child.Key, child.Value ?? string.Empty);
                }
                ImGui.TreePop();
            }
        }

        private float _targetFixedUpdateTime = 0;

        private void DrawWorld()
        {
            _renderContext.Begin();
            _world.Draw(_renderContext);
            _world.DrawGui(_renderContext);
            _renderContext.End();
        }

        public void Dispose()
        {
            _world.Dispose();
        }

        internal void ResetCamera()
        {
            EditorHook.CurrentZoomLevel = EditorHook.STARTING_ZOOM;
            _renderContext.Camera.Zoom = 1;
            _renderContext.Camera.Position = Vector2.Zero;
        }

        internal void CenterCamera()
        {
            _renderContext.Camera.Position = -_renderContext.Camera.Size / 2f;
        }
        internal void CenterCamera(Vector2 size)
        {
            _renderContext.Camera.Position = -size / 2f;
        }
    }
}