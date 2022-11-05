using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Microsoft.Xna.Framework;
using Murder.Components;
using Murder.Core;
using Murder.Core.Graphics;
using Murder.Editor.Attributes;
using Murder.Services;
using Murder.Utilities;

namespace Murder.Editor.Systems
{
    [DoNotPause]
    [OnlyShowOnDebugView]
    [Filter(typeof(TileGridComponent))]
    public class TileEditorSystem : IMonoRenderSystem
    {
        public ValueTask Draw(RenderContext render, Context context)
        {
            foreach (Entity e in context.Entities)
            {
                Point position = new();
                if (e.TryGetPosition() is PositionComponent positionComponent)
                {
                    position = positionComponent.ToPoint();
                }

                TileGridComponent grid = e.GetTileGrid();

                RenderServices.DrawRectangleOutline(
                    render.DebugSpriteBatch, 
                    new Rectangle(position.X, position.Y, grid.Width * Grid.CellSize, grid.Height * Grid.CellSize),
                    Game.Profile.Theme.GenericAsset.ToXnaColor());
            }

            return default;
        }
    }
}
