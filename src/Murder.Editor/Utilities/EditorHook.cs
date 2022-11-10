using Bang.Components;
using Bang.Entities;
using Murder.Core.Geometry;

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

        public int Hovering;
        public readonly List<int> Selected = new();
        public bool DrawSelection = true;

        public Action? RefreshAtlas;
        public Action<Entity>? OnClickEntity;
        public Action<Entity>? OnHoverEntity;
        public Func<Entity, bool>? DrawEntityInspector;

        public Action<IComponent[]>? AddEntityWithStage;
        public Action<int, IComponent>? OnComponentModified;

        public Point Offset;
        public Vector2 StageSize;

        public bool ShowDebug = false;

        public bool DrawCollisions = true;
        public bool DrawGrid = false;
        public bool DrawPathfind = false;
        public bool ShowStates = false;

        public readonly float[] ScrollPositions = new float[] { 0.25f, 0.5f, 0.75f, 1f, 2f, 4f, 8f, 10f, 16f, 32f, 48f };
        public const int STARTING_ZOOM = 3; //4th position on the array (1f)
        public int CurrentZoomLevel = STARTING_ZOOM;

        public ShowQuadTree DrawQuadTree = ShowQuadTree.None;

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
