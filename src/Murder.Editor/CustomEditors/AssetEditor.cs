using Bang.Components;
using Bang.Contexts;
using ImGuiNET;
using Microsoft.Xna.Framework.Input;
using Murder.Assets;
using Murder.Components;
using Murder.Core;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using Murder.Editor.Assets;
using Murder.Editor.Components;
using Murder.Editor.Core;
using Murder.Editor.CustomComponents;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Stages;
using Murder.Editor.Utilities;
using Murder.Prefabs;
using Murder.Utilities;
using Murder.Utilities.Attributes;
using System;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Editor.CustomEditors
{
    public abstract class AssetEditor : CustomEditor
    {
        public const int RenderContextEditorScale = 6;

        protected GameAsset? _asset;

        public override GameAsset Target => _asset!;

        protected Dictionary<Guid, Stage> Stages { get; private set; } = new();

        /// <summary>
        /// Information persisted for an asset and its stage. This is discarded once the tab is closed.
        /// </summary>
        private readonly Dictionary<Guid, StageAssetInfo> _stageInfo = new();

        public bool ShowColliders
        {
            get => _showColliders;
            set
            {
                if (_showColliders == value)
                    return;

                _showColliders = value;
                foreach (var stage in Stages)
                {
                    stage.Value.EditorHook.DrawCollisions = value;
                }
            }
        }

        public bool ShowGrid
        {
            get => _showGrid;
            set
            {
                if (_showGrid == value)
                    return;

                _showGrid = value;
                foreach (Stage stage in Stages.Values)
                {
                    stage.EditorHook.DrawGrid = value;
                }
            }
        }

        public bool ShowReflection
        {
            get => _showReflection;
            set
            {
                if (_showReflection == value)
                    return;

                _showReflection = value;
                foreach (var stage in Stages)
                {
                    stage.Value.EditorHook.ShowReflection = value;
                }
            }
        }

        public override IEntity? SelectedEntity => SelectedEntityImpl;

        /// <summary>
        /// Exposes the entity currently selected in this editor.
        /// </summary>
        protected abstract IEntity? SelectedEntityImpl { get; }

        private bool _showReflection = true;
        private bool _showColliders = true;
        private bool _showGrid = false;

        private string _tempRename = "";

        private Guid _draggedChildren = Guid.Empty;

        public override void OpenEditor(ImGuiRenderer imGuiRenderer, RenderContext renderContext, object target, bool overwrite)
        {
            Guid targetGuid = ((GameAsset)target).Guid;
            if (!overwrite)
            {
                if (targetGuid == _asset?.Guid || target is null)
                {
                    // No operation.
                    return;
                }
            }

            if (Architect.EditorSettings.StageInfo.TryGetValue(targetGuid, out PersistStageInfo info))
            {
                renderContext.Camera.Position = info.Position;
                renderContext.RefreshWindow(Architect.GraphicsDevice, info.Size, info.Size, new ViewportResizeStyle(ViewportResizeMode.None));
            }
            else if (target is not PrefabAsset)
            {
                renderContext.Camera.Position = Vector2.Zero;
            }

            _asset = (GameAsset)target;

            OnSwitchAsset(imGuiRenderer, renderContext, overwrite);
        }

        public override void Dispose()
        {
            foreach (Stage stage in Stages.Values)
            {
                stage.Dispose();
            }

            Stages.Clear();
        }

        protected virtual void OnSwitchAsset(ImGuiRenderer imGuiRenderer, RenderContext renderContext, bool forceInit) { }

        protected virtual void InitializeStage(Stage stage, Guid guid)
        {
            Stages[guid] = stage;

            Stages[guid].EditorHook.OnComponentModified += OnEntityModified;
            Stages[guid].EditorHook.DrawCollisions = _showColliders;

            _stageInfo[guid] = new();
        }

        /// <summary>
        /// Draw an entity to the editor.
        /// This allows to list all the components+children and to manipulate them.
        /// </summary>
        /// <param name="entityInstance">
        /// The target instance that will be drawn.
        /// </param>
        /// <param name="parent">
        /// This is the parent which will be modified for any of its children operations.
        /// This will be the root of the tree of entities which can be modified, not necessarily the parent.
        /// </param>
        protected void DrawEntity(IEntity entityInstance, bool canBeColapsed, IEntity? parent = null)
        {
            ImGui.BeginGroup();

            DrawEntityContent(entityInstance, canBeColapsed, parent);

            ImGui.EndGroup();

            var padding = ImGui.GetStyle().DisplaySafeAreaPadding;
            var p1 = ImGui.GetItemRectMin() + new Vector2(-padding.X, 0);
            var p2 = new Vector2(ImGui.GetItemRectSize().X + padding.X * 4.5f, ImGui.GetItemRectSize().Y);
            if (canBeColapsed)
                ImGui.GetWindowDrawList().AddRect(p1, p1 + p2, ImGuiHelpers.MakeColor32(Game.Profile.Theme.BgFaded), ImGui.GetStyle().FrameRounding);
        }

        protected void DrawEntityContent(IEntity entityInstance, bool canBeCollapsed, IEntity? parent = null)
        {
            GameLogger.Verify(Stages is not null);
            GameLogger.Verify(_asset is not null);

            if (entityInstance.PrefabRefName is string name)
            {
                if (canBeCollapsed)
                {
                    // This actually doesn't do what we think it does.
                    // ImGui.SameLine();
                }

                if (Architect.Instance.ActiveScene is EditorScene editorScene && entityInstance is PrefabEntityInstance prefabEntityInstance)
                {
                    ImGui.TextColored(Game.Profile.Theme.Faded, $"Instance of");
                    ImGui.SameLine();
                    if (ImGui.TextLink($"{name}"))
                    {
                        if (prefabEntityInstance.PrefabRef.CanFetch && prefabEntityInstance.PrefabRef.Fetch() is PrefabAsset asset)
                        {
                            editorScene.OpenOnTreeView(asset, true);
                            editorScene.OpenAssetEditor(asset, false);
                        }
                    }
                    ImGuiHelpers.HelpTooltip("Open original in a new tab");

                    ImGui.Separator();
                }
                else
                {
                    ImGui.TextColored(Game.Profile.Theme.Faded, $" Instance of '{name}'");
                }
            }

            //ImGui.BeginChild($"entity_{entityInstance.Guid}", new System.Numerics.Vector2(-1, 0), true, ImGuiWindowFlags.AlwaysAutoResize);

            // Only instance assets can be collapsed.
            if (canBeCollapsed)
            {
                //ImGui.GetWindowDrawList().AddText(p0 + new Vector2(-18, 24), 0x88FFFFFF, "\uf406");

                ImGui.PushStyleColor(ImGuiCol.Header, Game.Profile.Theme.Foreground);
                if (!ImGui.TreeNodeEx(entityInstance.Name, ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.Framed | ImGuiTreeNodeFlags.SpanAvailWidth))
                {
                    ImGui.PushStyleColor(ImGuiCol.Header, Game.Profile.Theme.BgFaded);

                    ImGui.PopStyleColor();
                    return;
                }
                ImGui.PopStyleColor();

            }

            if (entityInstance.Name is not null)
            {
                EntityInstance? instance = entityInstance as EntityInstance;
                if (instance is not null)
                {
                    bool active = !instance.IsDeactivated;
                    ImGui.PushStyleColor(ImGuiCol.Text, active ? Architect.Profile.Theme.White : Architect.Profile.Theme.Faded);
                    if (ImGui.Checkbox($"##{entityInstance.Guid}", ref active))
                    {
                        EnableEntity(parent?.Guid, instance, active);
                    }
                    ImGuiHelpers.HelpTooltip("Entity is enabled on runtime");
                    ImGui.PopStyleColor();
                    ImGui.SameLine();
                }

                if (CanDeleteInstance(parent, entityInstance))
                {
                    if (ImGuiHelpers.DeleteButton($"Delete_{entityInstance.Guid}"))
                    {
                        DeleteInstance(parent, entityInstance.Guid);

                        return;
                    }

                    ImGui.SameLine();
                }

                if (instance is not null)
                {
                    char icon = _stageInfo[_asset.Guid].InvisibleEntities.Contains(entityInstance.Guid) ? '\uf070' : '\uf06e';
                    if (ImGuiHelpers.IconButton(icon, $"hide_{entityInstance.Guid}"))
                    {
                        SwitchInstanceVisibility(parent, instance);
                    }

                    ImGuiHelpers.HelpTooltip("Hide on map");
                    ImGui.SameLine();
                }

                if (instance is not null && parent is null && entityInstance is not PrefabAsset)
                {
                    if (ImGuiHelpers.IconButton('\uf24d', $"duplicate_{entityInstance.Guid}"))
                    {
                        Duplicate(instance);
                    }

                    ImGuiHelpers.HelpTooltip("Duplicate");
                    ImGui.SameLine();
                }

                // Do not modify the name for entity assets, only instances.
                if (entityInstance is not PrefabAsset)
                {
                    if (ImGuiHelpers.IconButton('\uf304', $"rename_{entityInstance.Guid}"))
                    {
                        ImGui.OpenPopup($"Rename#{entityInstance.Guid}");
                    }

                    ImGuiHelpers.HelpTooltip("Rename");
                }

                if (instance is not null && parent is null && entityInstance is not PrefabEntityInstance)
                {
                    ImGui.SameLine(); 
                    
                    if (ImGuiHelpers.IconButton('\uf5c2', $"create_{entityInstance.Guid}"))
                    {
                        DrawTurnIntoPrefab(parent, instance);
                    }

                    ImGuiHelpers.HelpTooltip("Turn into a prefab");
                }

                if (ImGui.BeginPopup($"Rename#{entityInstance.Guid}"))
                {
                    if (DrawRenameInstanceModal(parent, entityInstance))
                    {
                        ImGui.CloseCurrentPopup();
                    }

                    ImGui.EndPopup();
                }
            }

            // Order by components name.
            var components = GetComponents(parent, entityInstance);
            if (components.Length == 0)
            {
                ImGui.TextColored(Game.Profile.Theme.Faded, "<No components>");
                ImGui.Dummy(new Vector2(10, 10));
            }

            // Draw components!
            ImGui.BeginGroup();
            {
                foreach (IComponent c in components)
                {
                    Type t = c.GetType();

                    // Check if this is an aseprite component.
                    // This defines whether we will draw it on the stage.
                    bool isSprite = t == typeof(SpriteComponent);
                    bool isCollider = t == typeof(ColliderComponent);
                    bool isOpen = false;

                    Type tTargetType = t.IsGenericType ? t.GetGenericArguments()[0] : t;

                    AttributeExtensions.TryGetAttribute<CustomNameAttribute>(tTargetType, out var customName);
                    string componentName = customName?.Name ?? ReflectionHelper.GetGenericName(t);

                    // Add an icon indicating that this component actually has a sound.
                    bool isSound = Attribute.IsDefined(tTargetType, typeof(SoundPlayerAttribute));
                    if (isSound)
                    {
                        componentName += " \uf001";
                    }

                    bool canRevert = CanRevertComponent(parent, entityInstance, t);
                    if (canRevert)
                    {
                        ImGui.PushStyleColor(ImGuiCol.HeaderHovered, Architect.Profile.Theme.Red);
                        ImGui.PushStyleColor(ImGuiCol.HeaderActive, Architect.Profile.Theme.RedFaded);
                        ImGui.PushStyleColor(ImGuiCol.Header, Architect.Profile.Theme.RedFaded);
                    }
                    else
                    {
                        ImGui.PushStyleColor(ImGuiCol.HeaderHovered, Architect.Profile.Theme.Accent);
                        ImGui.PushStyleColor(ImGuiCol.HeaderActive, Architect.Profile.Theme.HighAccent);
                        ImGui.PushStyleColor(ImGuiCol.Header, Architect.Profile.Theme.BgFaded);
                    }

                    // Draw the component

                    ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 10);
                    if (ImGuiHelpers.DeleteButton($"Delete_{t}"))
                    {
                        RemoveComponent(parent, entityInstance, t);
                    }

                    if (canRevert)
                    {
                        ImGui.SameLine();
                        if (ImGuiHelpers.IconButton('\uf1da', $"revert_{t}", sameLine: true, tooltip: "Revert"))
                        {
                            RevertComponent(parent, entityInstance, t);
                        }
                    }

                    ImGui.PopStyleVar();

                    ImGui.SameLine();
                    bool open = ImGui.TreeNodeEx(componentName, ImGuiTreeNodeFlags.Framed | ImGuiTreeNodeFlags.SpanAvailWidth);
                    if (open)
                    {
                        ImGui.TreePop();
                    }


                    if (open)
                    {

                        // TODO: This is modifying the memory of all readonly structs.
                        IComponent copy = SerializationHelper.DeepCopy(c);
                        if (CustomComponent.ShowEditorOf(ref copy))
                        {
                            ActWithUndo(
                                @do: () => ReplaceComponent(parent, entityInstance, copy),
                                undo: () => ReplaceComponent(parent, entityInstance, c));
                        }

                        isOpen = true;
                    }

                    ImGui.PopStyleColor(3);

                    if (!Stages.ContainsKey(_asset.Guid))
                    {
                        // Stage not initialized?
                        return;
                    }

                    // Do leftover stuff
                    if (isSprite)
                    {
                        if (isOpen)
                        {
                            Stages[_asset.Guid].AddComponentForInstance(parent?.Guid, entityInstance.Guid, new ShowYSortComponent());
                        }
                        else
                        {
                            Stages[_asset.Guid].RemoveComponentForInstance(parent?.Guid, entityInstance.Guid, typeof(ShowYSortComponent));
                        }
                    }

                    if (isCollider)
                    {
                        if (isOpen)
                        {
                            Stages[_asset.Guid].AddComponentForInstance(parent?.Guid, entityInstance.Guid, new ShowColliderHandlesComponent());
                        }
                        else
                        {
                            Stages[_asset.Guid].RemoveComponentForInstance(parent?.Guid, entityInstance.Guid, typeof(ShowColliderHandlesComponent));
                        }
                    }
                }
            }
            ImGui.EndGroup();


            ImGui.Separator();

            if (!entityInstance.HasComponent(typeof(ITransformComponent)))
            {
                if (ImGui.Button("+\uf0b2"))
                {
                    AddComponent(parent, entityInstance, typeof(PositionComponent));
                }
                ImGuiHelpers.HelpTooltip("Add PositionComponent");
                ImGui.SameLine();
            }

            if (!entityInstance.HasComponent(typeof(SpriteComponent)))
            {
                if (ImGui.Button("+\uf03e"))
                {
                    AddComponent(parent, entityInstance, typeof(SpriteComponent));
                }
                ImGuiHelpers.HelpTooltip("Add SpriteComponent");
                ImGui.SameLine();
            }

            if (!entityInstance.HasComponent(typeof(ColliderComponent)))
            {
                if (ImGui.Button("+\uf0c8"))
                {
                    if (!entityInstance.HasComponent(typeof(PositionComponent)))
                    {
                        AddComponent(parent, entityInstance, typeof(PositionComponent));
                    }
                    AddComponent(parent, entityInstance, typeof(ColliderComponent));
                }
                ImGuiHelpers.HelpTooltip("Add ColliderComponent");
                ImGui.SameLine();
            }

            if (entityInstance.Components.Length <= 1)
            {
                if (ImGui.Button("+\uf001"))
                {
                    AddComponent(parent, entityInstance, typeof(SoundShapeComponent));
                    AddComponent(parent, entityInstance, typeof(AmbienceComponent));
                }

                ImGuiHelpers.HelpTooltip("Add sound events on area...");
                ImGui.SameLine();
            }

            Type? newComponentToAdd = SearchBox.SearchComponent(entityInstance.Components);
            if (newComponentToAdd is not null)
            {
                AddComponent(parent, entityInstance, newComponentToAdd);
            }

            // --- Draw children! ---
            ImGui.BeginGroup();
            ImGui.Dummy(new System.Numerics.Vector2(0, 5));
            if (entityInstance.Children.Length > 0)
            {
                ImGuiHelpers.ColorIcon('\uf1ae', Game.Profile.Theme.White);
                ImGui.PushStyleColor(ImGuiCol.Text, Game.Profile.Theme.White);
            }
            else
            {
                ImGuiHelpers.ColorIcon('\uf192', Game.Profile.Theme.Faded);
                ImGui.PushStyleColor(ImGuiCol.Text, Game.Profile.Theme.Faded);
            }

            ImGui.SameLine();
            bool unfolded = false;
            if (ImGui.TreeNode("Children"))
            {
                unfolded = true;
                ImGui.PopStyleColor();
                // Always support adding more children...
                Guid? targetChild = default;

                if (ImGui.Button("Add Empty Child"))
                {
                    targetChild = Guid.Empty;
                }
                ImGui.SameLine();
                ImGui.Text("or");
                ImGui.SameLine();
                if (SearchBox.SearchInstantiableEntities(entityInstance) is Guid asset)
                {
                    targetChild = asset;
                }

                if (targetChild is not null)
                {
                    EntityInstance childInstance = EntityBuilder.CreateInstance(targetChild.Value, $"Child {entityInstance.Children.Length}");
                    AddChild(parent, entityInstance, childInstance);
                }

                IEntity? parentForChildren;
                if (parent is PrefabEntityInstance)
                {
                    // Now, this is the actually tricky part. If we currently hold a PrefabEntityInstance,
                    // we will guard all the children from here and pass ourselves as the parent of the child.
                    parentForChildren = parent;
                }
                else
                {
                    // Otherwise, move on.
                    parentForChildren = entityInstance;
                }

                CreateDropArea(0);

                ImmutableArray<EntityInstance> children = GetChildren(parent, entityInstance);
                for (int i = 0; i < children.Length; i++)
                {
                    EntityInstance childInstance = children[i];

                    // Draw drag and drop.
                    ImGui.BeginGroup();
                    //ImGui.Dummy(new Vector2(0, 5));
                    ImGui.PushID($"{childInstance.Guid}");

                    DrawEntity(childInstance, true, parentForChildren);

                    ImGui.PopID();
                    ImGui.EndGroup();

                    if (ImGui.BeginDragDropSource())
                    {
                        ImGui.Text($"Moving '{childInstance.Name}'...");

                        ImGui.SetDragDropPayload("Child", IntPtr.Zero, 0);
                        _draggedChildren = entityInstance.Guid;

                        ImGui.EndDragDropSource();
                    }

                    if (ImGui.BeginDragDropTarget())
                    {
                        ImGuiPayloadPtr payload = ImGui.AcceptDragDropPayload("Child");

                        bool hasDropped;
                        unsafe
                        {
                            hasDropped = payload.NativePtr != null;
                        }

                        if (hasDropped)
                        {
                            GameLogger.Log($"Drooped {_draggedChildren} into {i}");
                        }

                        ImGui.EndDragDropTarget();
                    }

                    CreateDropArea(i + 1);
                }

                ImGui.TreePop();
            }
            else
            {
                ImGui.PopStyleColor();
                ImGui.Dummy(new Vector2(5, 5));
            }

            if (entityInstance is not PrefabAsset)
            {
                // We only open a tree node if the entity is an instance.
                ImGui.TreePop();
            }
            ImGui.EndGroup();
            if (unfolded)
            {
                var padding = ImGui.GetStyle().DisplaySafeAreaPadding;
                var p1 = ImGui.GetItemRectMin() + new Vector2(-padding.X, 0);
                var p2 = new Vector2(ImGui.GetItemRectSize().X + padding.X * 2.25f, ImGui.GetItemRectSize().Y);
                ImGui.GetWindowDrawList().AddRect(p1, p1 + p2, ImGuiHelpers.MakeColor32(Game.Profile.Theme.BgFaded), ImGui.GetStyle().FrameRounding);
            }
        }

        private void CreateDropArea(int i)
        {
            ImGui.Dummy(new(ImGui.GetContentRegionAvail().X, 10));
            if (ImGui.BeginDragDropTarget())
            {
                ImGuiPayloadPtr payload = ImGui.AcceptDragDropPayload("Child");

                bool hasDropped;
                unsafe
                {
                    hasDropped = payload.NativePtr != null;
                }

                if (hasDropped)
                {
                    GameLogger.Log($"Drooped {_draggedChildren} after {i}");
                }

                ImGui.EndDragDropTarget();
            }
        }

        public override void CloseEditor(Guid guid)
        {
            if (Stages.TryGetValue(guid, out Stage? stage))
            {
                stage.Dispose();
            }

            Stages.Remove(guid);
            _stageInfo.Remove(guid);

            if (_asset?.Guid == guid)
            {
                _asset = null;
            }
        }

        /// <summary>
        /// Returns whether we can delete the entity.
        /// This is usually the case if this is an instance entity or the parent (if any)
        /// allows to delete that instance.
        /// </summary>
        protected virtual bool CanDeleteInstance(IEntity? parent, IEntity entity)
        {
            if (parent is not null)
            {
                return parent.CanRemoveChild(entity.Guid);
            }

            return entity is EntityInstance;
        }

        private ImmutableArray<EntityInstance> GetChildren(IEntity? parent, IEntity entityInstance)
        {
            // First, acquire the children for the entity.
            ImmutableArray<EntityInstance> children;
            if (parent is PrefabEntityInstance prefabInstance)
            {
                children = prefabInstance.FetchChildChildren(entityInstance);
            }
            else
            {
                children = entityInstance.FetchChildren();
            }

            return children;
        }

        protected ImmutableArray<IComponent> GetComponents(IEntity? parent, IEntity entityInstance)
        {
            // First, acquire the components for the entity.
            ImmutableArray<IComponent> components;
            if (parent is not null)
            {
                components = parent.GetChildComponents(entityInstance.Guid);
            }
            else
            {
                components = entityInstance.Components;
            }

            var builder = ImmutableArray.CreateBuilder<IComponent>();

            // Place "Position" as the first component.
            if (entityInstance.HasComponent(typeof(ITransformComponent)) &&
                components.FirstOrDefault(c => c is ITransformComponent) is IComponent position)
            {
                builder.Add(position);
                components = components.Remove(position);
            }

            // Order by alphabetical order.
            builder.AddRange(components.OrderBy(c => c.GetType().Name));
            return builder.ToImmutable();
        }

        private void RevertComponent(IEntity? parent, IEntity entityInstance, Type t)
        {
            GameLogger.Verify(_asset is not null && Stages.ContainsKey(_asset.Guid));

            IComponent component;
            if (parent is not null)
            {
                parent.RevertComponentForChild(entityInstance.Guid, t);
                component = parent.TryGetComponentForChild(entityInstance.Guid, t)!;
            }
            else
            {
                entityInstance.RevertComponent(t);
                component = entityInstance.GetComponent(t);
            }

            Stages[_asset.Guid].ReplaceComponentForInstance(parent?.Guid, entityInstance.Guid, component);
            _asset.FileChanged = true;
        }

        protected void ReplaceComponent(IEntity? parent, IEntity entityInstance, IComponent c)
        {
            GameLogger.Verify(_asset is not null && Stages.ContainsKey(_asset.Guid));

            if (parent is not null)
            {
                parent.AddOrReplaceComponentForChild(entityInstance.Guid, c);
            }
            else
            {
                entityInstance.AddOrReplaceComponent(c);
            }

            Stages[_asset.Guid].ReplaceComponentForInstance(parent?.Guid, entityInstance.Guid, c);
            _asset.FileChanged = true;
        }

        protected void RemoveComponent(IEntity? parent, IEntity entityInstance, Type t)
        {
            GameLogger.Verify(_asset is not null && Stages.ContainsKey(_asset.Guid));

            if (parent is not null)
            {
                parent.RemoveComponentForChild(entityInstance.Guid, t);
            }
            else
            {
                entityInstance.RemoveComponent(t);
            }

            Stages[_asset.Guid].RemoveComponentForInstance(parent?.Guid, entityInstance.Guid, t);
            _asset.FileChanged = true;
        }

        protected void EnableEntity(Guid? parentGuid, EntityInstance entityInstance, bool enable)
        {
            GameLogger.Verify(_asset is not null && Stages.ContainsKey(_asset.Guid));

            entityInstance.IsDeactivated = !enable;
            _asset.FileChanged = true;

            StageAssetInfo info = _stageInfo[_asset.Guid];
            if (info.InvisibleEntities.Contains(entityInstance.Guid))
            {
                return;
            }

            Stage targetStage = Stages[_asset.Guid];
            if (enable)
            {
                targetStage.ActivateInstance(parentGuid, entityInstance.Guid);
            }
            else
            {
                targetStage.DeactivateInstance(parentGuid, entityInstance.Guid);
            }
        }

        private void AddChild(IEntity? parent, IEntity entityInstance, EntityInstance child)
        {
            GameLogger.Verify(_asset is not null && Stages.ContainsKey(_asset.Guid));

            if (parent is PrefabEntityInstance prefabInstance && !prefabInstance.CanModifyChildAt(entityInstance.Guid))
            {
                // If we are guarding a prefab entity instance, make sure we add it from there.
                prefabInstance.AddChildAtChild(entityInstance.Guid, child);
            }
            else
            {
                // Otherwise, we are safe to mess with whatever reference we have directly.
                entityInstance.AddChild(child);
            }

            Stages[_asset.Guid].AddChildForInstance(parent?.Guid, entityInstance, child);
            _asset.FileChanged = true;
        }

        protected virtual bool CanRevertComponent(IEntity? parent, IEntity e, Type type)
        {
            // Is this a grandchild?
            if (parent is PrefabEntityInstance prefabInstance)
            {
                return prefabInstance.CanRevertComponentForChild(e.Guid, type);
            }

            // Otherwise, check for the entity itself.
            if (e.CanRevertComponent(type))
            {
                return true;
            }

            return false;
        }

        protected virtual void SwitchInstanceVisibility(IEntity? parent, EntityInstance entityInstance)
        {
            GameLogger.Verify(_asset is not null && Stages.ContainsKey(_asset.Guid));

            Stage targetStage = Stages[_asset.Guid];
            StageAssetInfo info = _stageInfo[_asset.Guid];

            Guid guid = entityInstance.Guid;
            if (info.InvisibleEntities.Contains(guid))
            {
                if (!entityInstance.IsDeactivated)
                {
                    ShowInstanceInEditor(parent?.Guid, guid);
                }

                info.InvisibleEntities.Remove(guid);
            }
            else
            {
                HideInstanceInEditor(parent?.Guid, guid);
            }
        }

        protected virtual void ShowInstanceInEditor(Guid? parent, Guid guid)
        {
            GameLogger.Verify(_asset is not null && Stages.ContainsKey(_asset.Guid));

            Stage targetStage = Stages[_asset.Guid];
            StageAssetInfo info = _stageInfo[_asset.Guid];

            targetStage.ActivateInstance(parent, guid);
            info.InvisibleEntities.Remove(guid);
        }

        protected virtual void HideInstanceInEditor(Guid? parent, Guid guid)
        {
            GameLogger.Verify(_asset is not null && Stages.ContainsKey(_asset.Guid));

            Stage targetStage = Stages[_asset.Guid];
            StageAssetInfo info = _stageInfo[_asset.Guid];

            targetStage.DeactivateInstance(parent, guid);
            info.InvisibleEntities.Add(guid);
        }

        protected virtual void DeleteInstance(IEntity? parent, Guid instanceGuid)
        {
            GameLogger.Verify(_asset is not null && Stages.ContainsKey(_asset.Guid));

            if (parent is not null)
            {
                parent.RemoveChild(instanceGuid);
            }

            Stages[_asset.Guid].RemoveInstance(parent?.Guid, instanceGuid);
            _asset.FileChanged = true;
        }

        /// <summary>
        /// Add a new component of type <paramref name="componentType"/> and all of its
        /// required components.
        /// </summary>
        protected void AddComponent(IEntity? parent, IEntity entityInstance, Type componentType)
        {
            GameLogger.Verify(_asset is not null && Stages.ContainsKey(_asset.Guid));

            var component = Activator.CreateInstance(componentType) as IComponent;

            GameLogger.Verify(component != null, "Error when creating a new component!");

            if (parent is not null)
            {
                parent.AddOrReplaceComponentForChild(entityInstance.Guid, component);
            }
            else
            {
                entityInstance.AddOrReplaceComponent(component);
            }

            Stages[_asset.Guid].AddComponentForInstance(parent?.Guid, entityInstance.Guid, component);
            _asset.FileChanged = true;

            // If this component require other components, make sure we also add all of them (if not already present).
            if (AttributeExtensions.TryGetAttribute(componentType, out RequiresAttribute? requiresAttribute))
            {
                foreach (Type requiredComponentType in requiresAttribute.Types)
                {
                    bool hasComponent = (parent is not null && parent.HasComponentAtChild(entityInstance.Guid, requiredComponentType)) ||
                        entityInstance.HasComponent(requiredComponentType);

                    if (!hasComponent)
                    {
                        Type t = requiredComponentType;

                        // TODO: Support generic interface components...?
                        if (t == typeof(ITransformComponent))
                        {
                            t = typeof(PositionComponent);
                        }

                        AddComponent(parent, entityInstance, t);
                    }
                }
            }
        }

        protected bool DrawRenameInstanceModal(IEntity? parent, IEntity e)
        {
            if (ImGui.IsWindowAppearing())
                _tempRename = e.Name;

            if (ImGui.IsWindowAppearing())
                ImGui.SetKeyboardFocusHere();
            ImGui.InputText($"##newName_{e.Guid}", ref _tempRename, 128, ImGuiInputTextFlags.AutoSelectAll | ImGuiInputTextFlags.AlwaysOverwrite);

            bool isValidName = parent is null || (parent is not null && !parent.FetchChildren().Any(child => e != child && child.Name == _tempRename));

            if (isValidName)
            {
                if (ImGui.Button("Ok!") || Game.Input.Pressed(Keys.Enter))
                {
                    e.SetName(_tempRename);
                    return true;
                }
            }
            else
            {
                ImGuiHelpers.DisabledButton("Ok!");
                ImGui.TextColored(Game.Profile.Theme.Warning, "Child cannot have the same name as its siblings!");
            }

            return false;
        }

        protected virtual void Duplicate(EntityInstance instance) {}

        protected bool DrawTurnIntoPrefab(IEntity? parent, EntityInstance instance)
        {
            if (parent is not null && instance.Children.Length > 0)
            {
                GameLogger.Warning("Making a prefab from an instance with a parent *and* children is not supported yet.");
                return false;
            }

            PositionComponent? position = null;
            if (instance.HasComponent(typeof(PositionComponent)))
            {
                position = (PositionComponent)instance.GetComponent(typeof(PositionComponent));
            }

            PrefabAsset prefabAsset = new(instance);
            if (position is not null)
            {
                // The instance previously had a position, let's set it to zero.
                prefabAsset.AddOrReplaceComponent(new PositionComponent());
            }

            Architect.EditorData.AddAsset(prefabAsset);

            if (Architect.Instance.ActiveScene is EditorScene scene)
            {
                scene.OpenAssetEditor(prefabAsset, overwrite: false);
            }

            bool modified = false;
            EntityInstance newInstance = EntityBuilder.CreateInstance(prefabAsset.Guid, instanceGuid: instance.Guid);
            if (position is not null)
            {
                // Replace the position with whatever we had before for this instance.
                newInstance.AddOrReplaceComponent(position);
            }

            if (parent is null)
            {
                if (UpdateEntityInstanceReferences(newInstance))
                {
                    return true;
                }
            }
            else
            {
                parent.AddChild(newInstance);
            }
            
            if (modified)
            {
                // If the world is saved, also save this now.
                _asset?.TrackAssetOnSave(prefabAsset.Guid);
            }

            return modified;
        }

        /// <summary>
        /// This will replace an entity instance with a particular instance to all references of <see cref="EntityInstance.Guid"/>.
        /// </summary>
        protected virtual bool UpdateEntityInstanceReferences(EntityInstance e)
        {
            GameLogger.Warning("This editor did not implement AssignEntityAs to modify this entity.");
            return false;
        }

        /// <summary>
        /// Tracks that an entity has been modified from the world.
        /// </summary>
        protected virtual void OnEntityModified(int entityId, Type t, IComponent? c)
        {
            GameLogger.Verify(_asset is not null && Stages.ContainsKey(_asset.Guid));

            // Do not persist reparenting!
            if (c is IParentRelativeComponent relativeComponent)
            {
                c = relativeComponent.WithoutParent();
            }

            Stage stage = Stages[_asset.Guid];
            if (stage.FindInstance(entityId) is IEntity entity)
            {
                IComponent? componentBefore = entity.HasComponent(t) ? entity.GetComponent(t) : null;
                if (componentBefore is not null && c is null)
                {
                    // remove component
                    ActWithUndo(
                        @do: () =>
                        {
                            entity.RemoveComponent(t);
                            _asset.FileChanged = true;
                        },
                        @undo: () =>
                        {
                            ReplaceComponent(parent: null, entity, componentBefore);
                        });
                }
                else if (componentBefore is null && c is not null)
                {
                    // add component
                    ActWithUndo(
                        @do: () =>
                        {
                            entity.AddOrReplaceComponent(c);
                            _asset.FileChanged = true;
                        },
                        @undo: () =>
                        {
                            RemoveComponent(parent: null, entity, t);
                        });
                }
                else if (componentBefore is not null && c is not null &&
                    !IComponent.Equals(componentBefore, c))
                {
                    // modified component
                    ActWithUndo(
                        @do: () =>
                        {
                            entity.AddOrReplaceComponent(c);
                            _asset.FileChanged = true;
                        },
                        @undo: () =>
                        {
                            ReplaceComponent(parent: null, entity, componentBefore);
                        });
                }
            }
            else if (stage.FindChildInstance(entityId) is (IEntity parent, Guid child))
            {
                IComponent? childComponent = parent.TryGetComponentForChild(child, t);
                if (childComponent is not null && c is null)
                {
                    // remove component
                    ActWithUndo(
                        @do: () =>
                        {
                            parent.RemoveComponentForChild(child, t);
                            _asset.FileChanged = true;
                        },
                        @undo: () =>
                        {
                            if (!parent.TryGetChild(child, out EntityInstance? instance))
                            {
                                return;
                            }

                            ReplaceComponent(parent, instance, childComponent);
                        });
                }
                else if (childComponent is null && c is not null)
                {
                    // add component
                    ActWithUndo(
                        @do: () =>
                        {
                            parent.AddOrReplaceComponentForChild(child, c);
                            _asset.FileChanged = true;
                        },
                        @undo: () =>
                        {
                            if (!parent.TryGetChild(child, out EntityInstance? instance))
                            {
                                return;
                            }

                            RemoveComponent(parent, instance, t);
                        });
                }
                else if (childComponent is not null && c is not null && !IComponent.Equals(childComponent, c))
                {
                    // modify component
                    ActWithUndo(
                        @do: () =>
                        {
                            parent.AddOrReplaceComponentForChild(child, c);
                            _asset.FileChanged = true;
                        },
                        @undo: () =>
                        {
                            if (!parent.TryGetChild(child, out EntityInstance? instance))
                            {
                                return;
                            }

                            ReplaceComponent(parent, instance, childComponent);
                        });
                }
            }
        }

        /// <summary>
        /// Add a new component of type to the *world only*.
        /// </summary>
        public void AddComponentsToWorldOnly(IEntity entityInstance, IComponent[] components)
        {
            GameLogger.Verify(_asset is not null && Stages.ContainsKey(_asset.Guid));

            foreach (IComponent c in components)
            {
                // TODO: Should we propagate a parent here? I guess children are unsupported.
                Stages[_asset.Guid].AddComponentForInstance(parentGuid: null, entityInstance.Guid, c);
            }
        }

        public void ToggleSystem(Type t, bool enable)
        {
            GameLogger.Verify(_asset is not null && Stages.ContainsKey(_asset.Guid));

            Stages[_asset.Guid].ToggleSystem(t, enable);
        }

        public void ActWithUndo(Action @do, Action undo)
        {
            if (_asset is null)
            {
                return;
            }

            bool wasAssetModified = _asset.FileChanged;
            @do.Invoke();

            void WrappedUndo()
            {
                if (_asset is null || !Stages.ContainsKey(_asset.Guid))
                {
                    return;
                }

                undo.Invoke();
                _asset.FileChanged = wasAssetModified;
            }

            Architect.Undo.Track(new UndoableAction(@do, WrappedUndo, addedAt: Game.NowUnscaled));
        }

        private class StageAssetInfo
        {
            public readonly HashSet<Guid> InvisibleEntities = new();

            public StageAssetInfo() { }
        }
    }
}