﻿using Bang.Components;
using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Utilities.Attributes;

namespace Murder.Components
{
    /// <summary>
    /// This component makes sure that any sprite will render as a 9-slice instead,
    /// as specified.
    /// </summary>
    public readonly struct NineSliceComponent : IComponent
    {
        [GameAssetId(typeof(SpriteAsset))]
        [Tooltip("Sprite which will be drawn as a nine slice.")]
        public readonly Guid Sprite = new();

        [Tooltip("Final size of the nine slice.")]
        public readonly Rectangle Target = Rectangle.Empty;

        [SpriteBatchReference]
        public readonly int TargetSpriteBatch = Batches2D.GameplayBatchId;

        public readonly int YSortOffset = 0;

        public NineSliceComponent() { }
    }
}
