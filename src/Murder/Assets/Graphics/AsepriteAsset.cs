using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Data;
using Murder.Utilities;
using Newtonsoft.Json;
using System.Collections.Immutable;

namespace Murder.Assets.Graphics
{
    public class AsepriteAsset : GameAsset, IPreview
    {
        [JsonProperty]
        public readonly AtlasId Atlas;
        [JsonProperty]
        public readonly ImmutableArray<string> Frames = ImmutableArray<string>.Empty;
        [JsonProperty]
        public readonly ImmutableDictionary<string, Animation> Animations = null!;
        [JsonProperty]
        public readonly Point Origin = new();
        [JsonProperty]
        public Point Size;

        public override char Icon => '\uf1fc';
        public override bool CanBeDeleted => false;
        public override bool CanBeRenamed => false;
        public override bool CanBeCreated => false;
        public override string EditorFolder => "#\uf085Generated";
        public override System.Numerics.Vector4 EditorColor => Game.Profile.Theme.Faded;

        [JsonConstructor]
        public AsepriteAsset() { }

        public AsepriteAsset(Guid guid, AtlasId atlas, string name, ImmutableArray<string> frames, ImmutableDictionary<string, Animation> animations, Point origin)
        {
            Guid = guid;
            Name = name;
            Atlas = atlas;
            Frames = frames;
            Animations = animations;
            Origin = origin;

            var img = Game.Data.FetchAtlas(atlas).Get(frames[0]);
            Size = img.OriginalSize;
        }

        public (AtlasId, string) GetPreviewId() => (Atlas, Frames[0]);

        public AtlasTexture GetFrame(int frame)
        {
            return Game.Data.FetchAtlas(Atlas).Get(Frames[frame]);
        }

        
    }
}
