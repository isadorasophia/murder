using ImGuiNET;
using Bang.Components;
using Microsoft.Xna.Framework.Input;
using System.Collections.Immutable;
using Murder.Assets;
using Murder.Prefabs;
using Murder.Diagnostics;
using Murder.Core.Geometry;
using Murder.ImGuiExtended;
using Murder.Components;
using Murder.Utilities;
using Murder.Editor.Utilities;
using Murder.Editor.Components;
using Murder.Editor.Stages;
using Murder.Editor.CustomComponents;
using Murder.Editor.ImGuiExtended;

namespace Murder.Editor.CustomEditors
{
    public abstract class AssetEditor : CustomEditor
    {
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

        private bool _showColliders = true;

        private string _tempRename = "";

        private readonly HashSet<Guid> _invisibleEntities = new();

        private Guid _draggedChildren = Guid.Empty;

        protected virtual void InitializeStage(Stage stage, Guid guid)
        {
            Stages[guid] = stage;
            Stages[guid].EditorHook.OnComponentModified += OnEntityModified;
            Stages[guid].EditorHook.DrawCollisions = _showColliders;
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
        protected void DrawEntity(IEntity entityInstance, IEntity? parent = null)
        {
            GameLogger.Verify(Stages is not null);
            GameLogger.Verify(_asset is not null);

            if (entityInstance.PrefabRefName is string name)
            {
                ImGui.SameLine();
                ImGui.TextColored(Game.Profile.Theme.Faded, $" Instance of '{name}'");
            }

            Vector2 padding = new(15, 15);
            Vector2 p0 = ImGui.GetCursorScreenPos();

            // Padding
            ImGui.Dummy(new Vector2(0, padding.Y));
            ImGui.Dummy(new Vector2(padding.X, 0));

            ImGui.BeginGroup();

            // Only instance assets can be collapsed.
            if (entityInstance is not PrefabAsset)
            {
                ImGui.GetWindowDrawList().AddText(FontAwesome.Big, 16, p0 + new Vector2(-18, 24), 0x88FFFFFF, "\uf406");

                if (!ImGui.TreeNodeEx(entityInstance.Name, ImGuiTreeNodeFlags.DefaultOpen))
                {
                    EndEntityGroup(padding, p0);
                    return;
                }
            }

            if (entityInstance.Name is not null)
            {
                if (CanDeleteInstance(parent, entityInstance))
                {
                    if (ImGuiHelpers.DeleteButton($"Delete_{entityInstance.Guid}"))
                    {
                        DeleteInstance(parent, entityInstance.Guid);

                        EndEntityGroup(padding, p0);
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
            }

            // Draw components!
            foreach (IComponent c in components)
            {
                Type t = c.GetType();

                // Check if this is an aseprite component.
                // This defines whether we will draw it on the stage.
                bool isAseprite = t == typeof(AsepriteComponent);
                bool isCollider = t == typeof(ColliderComponent);
                bool isOpen = false;

                if (ImGui.TreeNodeEx(ReflectionHelper.GetGenericName(t)))
                {
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

                    ImGui.TreePop();
                }

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

            if (!(entityInstance.HasComponent(typeof(ITransformComponent)) || entityInstance.HasComponent(typeof(RectPositionComponent))))
            {
                if (ImGui.Button("Add Position"))
                {
                    AddComponent(parent, entityInstance, typeof(PositionComponent));
                }
                ImGui.SameLine();
                if (ImGui.Button("Add RectPosition"))
                {
                    AddComponent(parent, entityInstance, typeof(RectPositionComponent));
                }
            }
            ImGui.Dummy(new Vector2(padding.X / 2f, 0));
            ImGui.SameLine();
            Type? newComponentToAdd = SearchBox.SearchComponent(entityInstance.Components);
            if (newComponentToAdd is not null)
            {
                AddComponent(parent, entityInstance, newComponentToAdd);
            }

            // --- Draw children! ---
            ImGui.Dummy(new System.Numerics.Vector2(0, 10));
            ImGuiHelpers.ColorIcon('\uf1ae', Game.Profile.Theme.White);
            ImGui.SameLine();

            if (ImGui.TreeNode("Children"))
            {
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

                    DrawEntity(childInstance, parentForChildren);

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

            if (entityInstance is not PrefabAsset)
            {
                // We only open a tree node if the entity is an instance.
                ImGui.TreePop();
            }

            EndEntityGroup(padding, p0);
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

        internal void RemoveStage(GameAsset closeTab)
        {
            Stages.Remove(closeTab.Guid);
        }

        private static void EndEntityGroup(Vector2 padding, Vector2 p0)
        {
            ImGui.Dummy(new Vector2(ImGui.GetContentRegionAvail().X - padding.X, 0));
            ImGui.EndGroup();

            Vector2 p1 = ImGui.GetItemRectMax() + new System.Numerics.Vector2(padding.X, padding.Y);

            ImGui.Dummy(new Vector2(0, padding.Y));
            var list = ImGui.GetWindowDrawList();
            list.AddRect(p0, p1, ImGuiHelpers.MakeColor32(Game.Profile.Theme.Faded), 16f);
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
            if (entityInstance.HasComponent(typeof(ITransformComponent)))
            {
                IComponent position = components.First(c => c is ITransformComponent);
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

        private void RemoveComponent(IEntity? parent, IEntity entityInstance, Type t)
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
        private void AddComponent(IEntity? parent, IEntity entityInstance, Type componentType)
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
                        AddComponent(parent, entityInstance, requiredComponentType);
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