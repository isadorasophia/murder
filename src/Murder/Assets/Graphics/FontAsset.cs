﻿using Murder.Attributes;
using Murder.Core.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Assets.Graphics
{
    public readonly struct Kerning
    {
        public Kerning()
        {
        }

        public int First { get; init; }
        public int Second { get; init; }
        public int Amount { get; init; }

    }
    public class FontAsset : GameAsset
    {
        public override string EditorFolder => "#Fonts";

        public string TexturePath = string.Empty;
        public int LineHeight = 0;
        public int Index = 0;
        public float Baseline;
        public readonly ImmutableDictionary<int, PixelFontCharacter> Characters = ImmutableDictionary<int, PixelFontCharacter>.Empty;

        [HideInEditor]
        public readonly ImmutableArray<Kerning> Kernings = ImmutableArray<Kerning>.Empty;

        public FontAsset(int index, Dictionary<int, PixelFontCharacter> characters, ImmutableArray<Kerning> kernings, int size, string texturePath, float baseline)
        {
            Index = index;
            Name = Path.GetFileNameWithoutExtension(texturePath);
            LineHeight = size;
            Baseline = baseline;
            TexturePath = Name + ".png";

            Characters = characters.ToImmutableDictionary();
            Kernings = kernings;
        }

        public FontAsset()
        {
        }
    }
}