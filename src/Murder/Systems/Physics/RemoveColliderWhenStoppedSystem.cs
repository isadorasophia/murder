using Bang;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using System.Collections.Immutable;

namespace Murder.Systems.Physics
{
    [Filter(typeof(RemoveColliderWhenStoppedComponent), typeof(ColliderComponent))]
    [Watch(typeof(VelocityComponent))]
    internal class RemoveColliderWhenStoppedSystem : IReactiveSystem
    {
        public void OnAdded(World world, ImmutableArray<Entity> entities)
        { }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        { }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            foreach (var e in entities)
            {
                e.RemoveCollider();
                e.RemoveRemoveColliderWhenStopped();
            }
        }
    }
}