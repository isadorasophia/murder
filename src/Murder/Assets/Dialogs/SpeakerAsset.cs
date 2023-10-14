﻿using Murder.Attributes;
using Murder.Core;
using System.Collections.Immutable;

namespace Murder.Assets
{
    public class SpeakerAsset : GameAsset
    {
        public override char Icon => '\uf2c1';
        public override string EditorFolder => "#\uf518Story\\#\uf2c1Speakers";

        [Tooltip("Name used on scripts and to reference this speaker")]
        public readonly string SpeakerName = string.Empty;

        [Tooltip("Portrait that will be shown by default, if none is specified.")]
        public readonly string? DefaultPortrait = "Idle";

        public readonly ImmutableDictionary<string, Portrait> Portraits = ImmutableDictionary<string, Portrait>.Empty;
    }
}