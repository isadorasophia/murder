using Bang;
using Microsoft.Xna.Framework;
using Murder.Components;
using Murder.Core.Particles;
using Murder.Prefabs;
using Murder.Utilities;

namespace Murder.Assets.Graphics
{
    public class ParticleSystemAsset : GameAsset
    {
        public override char Icon => '\ue2ca';
        
        public override string EditorFolder => "#\ue2caParticles";

        public readonly Emitter Emitter = new();

        public readonly Particle Particle = new();

        public PrefabAsset ToInstanceAsAsset(string name)
        {
            PrefabAsset asset = new(EntityBuilder.CreateInstance(Guid, name));
            asset.MakeGuid();

            return asset;
        }

        /// <summary>
        /// Create an instance of particle system.
        /// </summary>
        public int CreateAt(World world, Vector2 position, bool destroy)
        {
            ParticleSystemComponent c = new(Guid, destroy);
            return world.AddEntity(c, new PositionComponent(position.ToSysVector2())).EntityId;
        }
        
        public ParticleSystemComponent GetTrackerComponent()
        {
            return new(Guid, false);
        }
    }
}
