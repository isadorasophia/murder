using Bang.Components;
using Bang.Entities;
using Murder.Core.Geometry;
using System.Collections.Immutable;

namespace Murder.Editor.Utilities
{
    public class EditorHook
    {
        public enum ShowQuadTree
        {
            None,
            Collision,
            PushAway,
            Pathfind
        }

        public enum CursorStyle
        {
            Normal,
            Hand,
            Point,
            Eye
        }

        public Point CursorWorldPosition;
        public Point CursorScreenPosition;
        public CursorStyle Cursor = CursorStyle.Normal;

        private readonly HashSet<int> _hovering = new();
        
        private ImmutableArray<int>? _hoveringCache = default;

        public bool IsEntityHovered(int id) => _hovering.Contains(id);
        
        public ImmutableArray<int> Hovering => _hoveringCache ??= _hovering.ToImmutableArray();

        public void HoverEntity(Entity e)
        {
            _hovering.Add(e.EntityId);
            _hoveringCache = null;

            OnHoverEntity?.Invoke(e);
        }
        
        public void UnhoverEntity(Entity e)
        {
            _hovering.Remove(e.EntityId);
            _hoveringCache = null;
        }
        
        public Action<Entity>? OnHoverEntity;

        private readonly Dictionary<int, Entity> _select = new();

        private ImmutableDictionary<int, Entity>? _selectCache = default;

        public ImmutableDictionary<int, Entity> AllSelectedEntities => _selectCache ??= _select.ToImmutableDictionary();

        public bool IsEntitySelected(int id) => _select.ContainsKey(id);

        public void UnselectAll()
        {
            if (_select.Count == 0)
            {
                return;
            }

            foreach (Entity e in _select.Values)
            {
                UnselectEntity(e);
            }

            _select.Clear();
        }
        
        public void SelectEntity(Entity e, bool clear)
        {
            if (_select.ContainsKey(e.EntityId) && _select.Count == 1)
            {
                // Actually, do nothing.
                return;
            }
            
            if (clear)
            {
                UnselectAll();
            }
            
            _select[e.EntityId] = e;

            _selectCache = null;
            OnEntitySelected?.Invoke(e, true);
            
            OpenEntity(e);
        }
        
        public void UnselectEntity(Entity e)
        {
            _select.Remove(e.EntityId);
            _selectCache = null;
            
            OnEntitySelected?.Invoke(e, false);
        }

        /// <summary>
        /// Called whenever an entity is selected (or unselected!).
        /// </summary>
        public event Action<Entity, bool>? OnEntitySelected;

        private readonly HashSet<int> _openedEntities = new();

        private ImmutableArray<int>? _openedEntitiesCache = default;

        public ImmutableArray<int> AllOpenedEntities => _openedEntitiesCache ??= _openedEntities.ToImmutableArray();

        public bool IsEntityOpened(int id) => _openedEntities.Contains(id);

        public void OpenEntity(Entity e)
        {
            _openedEntities.Add(e.EntityId);
            _openedEntitiesCache = null;
            
            OnEntityOpened?.Invoke(e, true);
        }

        public void CloseEntity(Entity e)
        {
            _openedEntities.Remove(e.EntityId);
            _openedEntitiesCache = null;
            
            OnEntityOpened?.Invoke(e, false);

            UnselectEntity(e);
        }

        /// <summary>
        /// Called whenever an entity is opened (or unopened!).
        /// </summary>
        public event Action<Entity, bool>? OnEntityOpened;

        private Guid? _entityToBePlaced = null;
        
        /// <summary>
        /// Entity that it will be placed in the world with <see cref="WorldAssetEditor.DrawAllInstancesToAdd"/>.
        /// </summary>
        public Guid? EntityToBePlaced
        {
            get
            {
                return _entityToBePlaced;
            }
            set
            {
                _entityToBePlaced = value;
                
                if (value is not null)
                {
                    UnselectAll();
                }
            }
        }

        public bool DrawSelection = true;

        public Action? RefreshAtlas;
        public Func<Entity, bool>? DrawEntityInspector;
        
        public Action<Guid, IComponent[], string?>? AddPrefabWithStage;
        public Action<IComponent[], string?>? AddEntityWithStage;

        public Action<int>? RemoveEntityWithStage;
        public Action<int, IComponent>? OnComponentModified;
        
        public Point Offset;
        public Vector2 StageSize;

        public bool IsMouseOnStage =>
            new Rectangle(Offset, StageSize).Contains(Game.Input.CursorPosition);

        public bool ShowDebug = false;

        public bool DrawCollisions = true;
        public bool DrawGrid = false;
        public bool DrawPathfind = false;
        public bool ShowStates = false;
        public bool DrawTargetInteractions = false;

        public readonly float[] ScrollPositions = new float[] { 0.25f, 0.5f, 0.75f, 1f, 2f, 4f, 8f, 10f, 16f, 32f, 48f };
        public const int STARTING_ZOOM = 3; //4th position on the array (1f)
        public int CurrentZoomLevel = STARTING_ZOOM;

        /// <summary>
        /// Whenever a tile editor system is in place, this point to the tile currently selected.
        /// </summary>
        public int CurrentSelectedTile = 0;

        public ShowQuadTree DrawQuadTree = ShowQuadTree.None;
        
        /// <summary>
        /// Maps the groups according to the room entities.
        /// </summary>
        public readonly Dictionary<int, string> Groups = new();

        public string? TryGetGroupNameForEntity(int id) => Groups.TryGetValue(id, out string? name) ? name : null;

        /// <summary>
        /// Bound rectangles which will be displayed in the world.
        /// </summary>
        public Dictionary<Guid, Rectangle>? Dimensions { get; private set; }

        /// <summary>
        /// Add a dimension rectangle to the editor hook. This will be drawn in the world.
        /// </summary>
        public void AddEntityDimensionForEditor(Guid guid, Rectangle rect)
        {
            if (rect.Width == 0 && rect.Height == 0)
            {
                ClearDimension(guid);

                // Skip invalid rectangle dimensions.
                return;
            }

            Dimensions ??= new();

            Dimensions[guid] = rect;
        }

        /// <summary>
        /// Add a dimension rectangle to the editor hook. This will be drawn in the world.
        /// </summary>
        public void ClearDimension(Guid guid)
        {
            if (Dimensions is not null)
            {
                Dimensions.Remove(guid);
            }
        }
    }
}
