using Bang.Components;
using Murder.Attributes;
using Murder.Core.Graphics;
using Murder.Utilities.Attributes;

namespace Murder.Components
{
    public readonly struct DrawRectangleComponent : IComponent
    {
        [SpriteBatchReference]
        public readonly int TargetSpriteBatch = Batches2D.GameplayBatchId;

        public readonly bool Fill = false;
        public readonly int LineWidth = 1;

        [PaletteColor]
        public readonly Color Color = Color.Black;

        public readonly float SortingOffset = 0f;


        public DrawRectangleComponent() { }
    }
}