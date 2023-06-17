using ImGuiNET;
using Bang.Components;
using Microsoft.Xna.Framework.Input;
using System.Collections.Immutable;
using Murder.Assets;
using Murder.Prefabs;
using Murder.Diagnostics;
using Murder.Core.Geometry;
using Murder.Components;
using Murder.Utilities;
using Murder.Editor.Utilities;
using Murder.Editor.Components;
using Murder.Editor.Stages;
using Murder.Editor.CustomComponents;
using Murder.Editor.ImGuiExtended;
using Murder.Utilities.Attributes;
using Murder.Core.Graphics;
using Murder.Editor.Assets;

namespace Murder.Editor.CustomEditors
{
    public abstract class AssetEditor : CustomEditor
    {
        public const int RenderContextEditorScale = 6;

        protected Dictionary<Guid, Stage> Stages { get; private set; } = new();
        protected GameAsset? _asset;

        public override GameAsset Target => _asset!;

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

        public bool KeepColliderShapes
        {
            get => _keepColliderShapes;
            set
            {
                if (_keepColliderShapes == value)
                    return;

                _keepColliderShapes = value;
                foreach (var stage in Stages)
                {
                    stage.Value.EditorHook.KeepOriginalColliderShapes = value;
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

        /// <summary>
        /// Exposes the entity currently selected in this editor.
        /// </summary>
        public abstract IEntity? SelectedEntity { get; }

        private bool _showReflection = true;
        private bool _keepColliderShapes = true;
        private bool _showColliders = true;

        private string _tempRename = "";

        private readonly HashSet<Guid> _invisibleEntities = new();

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

            if (Architect.EditorSettings.CameraPositions.TryGetValue(targetGuid, out PersistStageInfo info))
            {
                renderContext.Camera.Position = info.Position;
                renderContext.RefreshWindow(info.Size, RenderContextEditorScale);
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
            Stages[guid].EditorHook.KeepOriginalColliderShapes = _keepColliderShapes;
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
            var padding = Vector2.Zero;// ImGui.GetStyle().DisplaySafeAreaPadding;
            var p1 = ImGui.GetItemRectMin() + new System.Numerics.Vector2(-padding.X, 0);
            var p2 = new System.Numerics.Vector2(ImGui.GetContentRegionAvail().X + padding.X, ImGui.GetItemRectSize().Y);
            
            if (canBeColapsed)
                ImGui.GetWindowDrawList().AddRect(p1, p1 + p2, ImGuiHelpers.MakeColor32(Game.Profile.Theme.BgFaded), ImGui.GetStyle().FrameRounding);
        }

        protected void DrawEntityContent(IEntity entityInstance, bool canBeColapsed, IEntity? parent = null)
        {
            GameLogger.Verify(Stages is not null);
            GameLogger.Verify(_asset is not null);

            if (entityInstance.PrefabRefName is string name)
            {
                if (canBeColapsed)
                    ImGui.SameLine();

                if (Architect.Instance.ActiveScene is EditorScene editorScene && entityInstance is PrefabEntityInstance prefabEntityInstance) {
                    ImGui.TextColored(Game.Profile.Theme.Faded, $" Instance of ");
                    ImGui.SameLine();
                    if (ImGui.SmallButton(name))
                    {
                        if (prefabEntityInstance.PrefabRef.CanFetch && prefabEntityInstance.PrefabRef.Fetch() is PrefabAsset asset)
                        {
                            editorScene.OpenAssetEditor(asset, false);
                        }
                    }
                }
                else
                {
                    ImGui.TextColored(Game.Profile.Theme.Faded, $" Instance of '{name}'");
                }
            }

            //ImGui.BeginChild($"entity_{entityInstance.Guid}", new System.Numerics.Vector2(-1, 0), true, ImGuiWindowFlags.AlwaysAutoResize);
            
            // Only instance assets can be collapsed.
            if (canBeColapsed)
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
                if (CanDeleteInstance(parent, entityInstance))
                {
                    if (ImGuiHelpers.DeleteButton($"Delete_{entityInstance.Guid}"))
                    {
                        DeleteInstance(parent, entityInstance.Guid);

                        return;
                    }

                    ImGui.SameLine();
                }

                if (entityInstance is EntityInstance instance)
                {
                    char icon = _invisibleEntities.Contains(entityInstance.Guid) ? '\uf070' : '\uf06e';
                    if (ImGuiHelpers.IconButton(icon, $"hide_{entityInstance.Guid}"))
                    {
                        SwitchInstanceVisibility(parent, instance);
                    }

                    ImGui.SameLine();
                }

                // Do not modify the name for entity assets, only instances.
                if (entityInstance is not PrefabAsset && ImGuiHelpers.IconButton('\uf304', $"rename_{entityInstance.Guid}"))
                {
                    ImGui.OpenPopup($"Rename#{entityInstance.Guid}");
                }

                if (ImGui.BeginPopup($"Rename#{entityInstance.Guid}"))
                {
                    if (DrawRenameInstanceModal(parent, entityInstance))
                        ImGui.CloseCurrentPopup();
                    ImGui.EndPopup();
                }
            }

            // Order by components name.
            var components = GetComponents(parent, entityInstance);
            if (components.Length == 0)
            {
                ImGui.TextColored(Game.Profile.Theme.Faded, "<No components>");
                ImGui.Dummy(new System.Numerics.Vector2(10, 10));
            }

            // Draw components!
            ImGui.BeginGroup();
            {
                foreach (IComponent c in components)
                {
                    Type t = c.GetType();

                    // Check if this is an aseprite component.
                    // This defines whether we will draw it on the stage.
                    bool isAseprite = t == typeof(SpriteComponent);
                    bool isCollider = t == typeof(ColliderComponent);
                    bool isOpen = false;

                    AttributeExtensions.TryGetAttribute<CustomNameAttribute>(t, out var customName);
                    string componentName = customName?.Name ?? ReflectionHelper.GetGenericName(t);

                    // Draw the component
                    if (ImGui.TreeNodeEx(componentName, ImGuiTreeNodeFlags.Framed | ImGuiTreeNodeFlags.SpanAvailWidth))
                    {
                        ImGui.TreePop();
                        if (ImGuiHelpers.DeleteButton($"Delete_{t}"))
                        {
                            RemoveComponent(parent, entityInstance, t);
                        }
                        else if (CanRevertComponent(parent, entityInstance, t) && ImGuiHelpers.IconButton('\uf1da', $"revert_{t}", sameLine: true))
                        {
                            RevertComponent(parent, entityInstance, t);
                        }
                        else
                        {
                            // TODO: This is modifying the memory of all readonly structs.
                            IComponent copy = SerializationHelper.DeepCopy(c);

                            if (CustomComponent.ShowEditorOf(copy))
                            {
                                // Asset was already modified, just pass along the updated asset.
                                ReplaceComponent(parent, entityInstance, copy);
                            }

                            isOpen = true;
                        }

                    }

                    if (!Stages.ContainsKey(_asset.Guid))
                    {
                        // Stage not initialized?
                        return;
                    }

                    // Do leftover stuff
                    if (isAseprite)
                    {
                        if (isOpen)
                        {
                            Stages[_asset.Guid].AddComponentForInstance(entityInstance.Guid, new ShowYSortComponent());
                        }
                        else
                        {
                            Stages[_asset.Guid].RemoveComponentForInstance(entityInstance.Guid, typeof(ShowYSortComponent));
                        }
                    }

                    if (isCollider)
                    {
                        if (isOpen)
                        {
                            Stages[_asset.Guid].AddComponentForInstance(entityInstance.Guid, new ShowColliderHandlesComponent());
                        }
                        else
                        {
                            Stages[_asset.Guid].RemoveComponentForInstance(entityInstance.Guid, typeof(ShowColliderHandlesComponent));
                        }
                    }
                }
            }
            ImGui.EndGroup();

            if (!(entityInstance.HasComponent(typeof(ITransformComponent)) || entityInstance.HasComponent(typeof(RectPositionComponent))))
            {
                if (ImGui.Button("Add Position"))
                {
                    AddComponent(parent, entityInstance, typeof(PositionComponent));
                }
                ImGui.SameLine();
                if (ImGui.Button("Add Basics"))
                {
                    AddComponent(parent, entityInstance, typeof(PositionComponent));
                    AddComponent(parent, entityInstance, typeof(SpriteComponent));
                    AddComponent(parent, entityInstance, typeof(ColliderComponent));
                }
            }
            Type? newComponentToAdd = SearchBox.SearchComponent(entityInstance.Components);
            if (newComponentToAdd is not null)
            {
                AddComponent(parent, entityInstance, newComponentToAdd);
            }

            // --- Draw children! ---
            ImGui.Dummy(new System.Numerics.Vector2(0, 10));
            if (entityInstance.Children.Length > 0)
            {
                ImGuiHelpers.ColorIcon('\uf1ae',  Game.Profile.Theme.White);
                ImGui.PushStyleColor(ImGuiCol.Text, Game.Profile.Theme.White);
            }
            else
            {
                ImGuiHelpers.ColorIcon('\uf192', Game.Profile.Theme.Faded);
                ImGui.PushStyleColor(ImGuiCol.Text, Game.Profile.Theme.Faded);
            }

            ImGui.SameLine();

            if (ImGui.TreeNode("Children"))
            {
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
                    ImGui.Dummy(new System.Numerics.Vector2(0, 5));
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
                ImGui.Dummy(new System.Numerics.Vector2(5, 5));
            }

            if (entityInstance is not PrefabAsset)
            {
                // We only open a tree node if the entity is an instance.
                ImGui.TreePop();
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

            Stages[_asset.Guid].ReplaceComponentForInstance(entityInstance.Guid, component);
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

            Stages[_asset.Guid].ReplaceComponentForInstance(entityInstance.Guid, c);
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

            Stages[_asset.Guid].RemoveComponentForInstance(entityInstance.Guid, t);
            _asset.FileChanged = true;
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

            Stages[_asset.Guid].AddChildForInstance(entityInstance, child);
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

            Guid guid = entityInstance.Guid;
            if (_invisibleEntities.Contains(guid))
            {
                if (parent is not null)
                {
                    targetStage.AddChildForInstance(parent, entityInstance);
                }
                else
                {
                    targetStage.AddEntity(entityInstance);
                }

                _invisibleEntities.Remove(guid);
            }
            else
            {
                targetStage.RemoveInstance(guid);
                _invisibleEntities.Add(guid);
            }
        }

        protected virtual void DeleteInstance(IEntity? parent, Guid instanceGuid)
        {
            GameLogger.Verify(_asset is not null && Stages.ContainsKey(_asset.Guid));

            if (parent is not null)
            {
                parent.RemoveChild(instanceGuid);
            }

            Stages[_asset.Guid].RemoveInstance(instanceGuid);
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

            Stages[_asset.Guid].AddComponentForInstance(entityInstance.Guid, component);
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

        protected virtual void OnEntityModified(int entityId, IComponent c)
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
                if (!entity.GetComponent(c.GetType()).Equals(c))
                {
                    entity.AddOrReplaceComponent(c);
                    
                    _asset.FileChanged = true;
                }
            }
            else if (stage.FindChildInstance(entityId) is (IEntity parent, Guid child))
            {
                if (parent.TryGetComponentForChild(child, c.GetType()) is IComponent childComponent &&
                    !childComponent.Equals(c))
                {
                    parent.AddOrReplaceComponentForChild(child, c);
                    _asset.FileChanged = true;
                }
            }
        }
    }
}