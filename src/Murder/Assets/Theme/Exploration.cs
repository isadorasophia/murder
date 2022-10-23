using Murder.Core.Graphics;

namespace Murder.Assets
{
    [Serializable]
    public class Exploration
    {
        public float TileExplorationDuration = 0.5f;
        public float SpriteExplorationDuration = 1.5f;

        public Color UnexploredColor = Color.CreateFrom256(31, 39, 61);
        public Color ExploreColor = Color.CreateFrom256(204, 154, 252);
    }
}
