using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Data;
using Murder.ImGuiExtended;
using System.Collections.Immutable;

namespace Murder.Assets.Graphics
{
    public class AsepriteAsset : GameAsset, IPreview
    {
        public override char Icon => '\uf1fc';
        public override bool CanBeDeleted => false;
        public override bool CanBeRenamed => false;
        public override bool CanBeCreated => false;
        public override string EditorFolder => "#\uf085Generated";
        public override System.Numerics.Vector4 EditorColor => Game.Profile.Theme.Faded;

        public ImmutableArray<string> Frames;
        public ImmutableDictionary<string, Animation> Animations = null!;
        public Point Origin = new();

        public AsepriteAsset(Guid guid, string name)
        {
            Guid = guid;
            Name = name;
        }

        public void DrawPreview()
        {
            ImGuiHelpers.Image(Frames[0], 256, Game.Data.FetchAtlas(AtlasId.Gameplay), Game.Instance.DPIScale / 100);
        }
    }
}
