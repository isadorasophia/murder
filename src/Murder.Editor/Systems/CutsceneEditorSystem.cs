using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components.Cutscenes;
using Murder.Core.Cutscenes;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Utilities;
using Murder.Services;
using Murder.Utilities;
using SharpDX.Mathematics.Interop;
using System.Collections.Immutable;

namespace Murder.Editor.Systems
{
    [CutsceneEditor]
    [Filter(typeof(CutsceneAnchorsComponent))]
    internal class CutsceneEditorSystem : IUpdateSystem, IMonoRenderSystem
    {
        private readonly Vector2 _selectionBox = new Point(12, 12);

        /// <summary>
        /// This is the position currently selected by the cursor.
        /// </summary>
        private Vector2? _selectPosition;

        private readonly Color _hoverColor = Game.Profile.Theme.White.WithAlpha(.7f);

        public ValueTask Update(Context context)
        {
            return default;
        }

        public ValueTask Draw(RenderContext render, Context context)
        {
            int lineWidth = Math.Max(Calculator.RoundToInt(2f / render.Camera.Zoom), 1);

            EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;
            foreach (Entity e in context.Entities)
            {
                Vector2 position = e.GetGlobalTransform().Vector2;

                ImmutableDictionary<string, Anchor> anchors = e.GetCutsceneAnchors().Anchors;
                RenderServices.DrawRectangleOutline(render.DebugFxSpriteBatch, FindContainingArea(position, anchors.Values), Game.Profile.Theme.White, 1);

                // Draw the root of the cutscene (possibly change for icon?)
                Rectangle hoverRectangle = new(position - _selectionBox / 2f, _selectionBox);
                RenderServices.DrawRectangle(render.DebugSpriteBatch, hoverRectangle, _hoverColor);

                foreach ((string name, Anchor anchor) in anchors)
                {
                    Vector2 anchorPosition = position + anchor.Position;
                    Rectangle anchorRectangle = new(anchorPosition - _selectionBox / 2f, _selectionBox);
                    RenderServices.DrawRectangle(render.DebugSpriteBatch, anchorRectangle, Game.Profile.Theme.Accent);

                    Game.Data.PixelFont.Draw(render.DebugSpriteBatch, name, lineWidth, anchorPosition, alignment: new Vector2(0.5f, -1), Color.White);
                }

                var distance = (position - hook.CursorWorldPosition).Length() / 128f * render.Camera.Zoom;
                if (distance < 1)
                {
                    RenderServices.DrawCircle(render.DebugSpriteBatch, position, 2, 6, Game.Profile.Theme.Yellow.WithAlpha(1 - distance));
                }
            }

            return default;
        }

        private Rectangle FindContainingArea(Vector2 initialPosition, IEnumerable<Anchor> anchors)
        {
            Vector2 minPoint = initialPosition;
            Vector2 maxPoint = initialPosition;

            foreach (Anchor anchor in anchors)
            {
                Vector2 anchorPosition = initialPosition + anchor.Position;

                if (anchorPosition.X < minPoint.X)
                {
                    minPoint.X = anchorPosition.X;
                }

                if (anchorPosition.Y < minPoint.Y)
                {
                    minPoint.Y = anchorPosition.Y;
                }

                if (anchorPosition.X > maxPoint.X)
                {
                    maxPoint.X = anchorPosition.X;
                }

                if (anchorPosition.Y > maxPoint.Y)
                {
                    maxPoint.Y = anchorPosition.Y;
                }
            }

            return GridHelper.FromTopLeftToBottomRight(minPoint, maxPoint);
        }
    }
}
