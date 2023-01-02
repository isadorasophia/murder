using Bang.Entities;
using Bang;
using Murder.Core.Particles;
using Murder.Prefabs;
using Murder.Components;
using Microsoft.Xna.Framework;

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
        public int CreateAt(World world, Vector2 position)
        {
            ParticleTracker tracker = new(Emitter, Particle);
            ParticleTrackerComponent trackerComponent = new(tracker);

            return world.AddEntity(trackerComponent, new PositionComponent(position)).EntityId;
        }
        
        public ParticleTrackerComponent GetTrackerComponent()
        {
            ParticleTracker tracker = new(Emitter, Particle);
            return new(tracker);
        }
    }
}
