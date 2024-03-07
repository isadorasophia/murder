using Bang;
using Bang.Components;
using Bang.Entities;
using Murder.Attributes;
using Murder.Utilities.Attributes;
using Newtonsoft.Json;
using System.Collections.Immutable;

namespace Murder.Components
{
    [RuntimeOnly, DoNotPersistOnSave]
    public readonly struct CollisionCacheComponent : IComponent
    {
        /// <summary>
        /// Id of the entity that caused this collision.
        /// </summary>
        [ShowInEditor]
        private readonly ImmutableHashSet<int> _collidingWith = ImmutableHashSet<int>.Empty;
        public CollisionCacheComponent(int id) => _collidingWith = ImmutableHashSet<int>.Empty.Add(id);
        public CollisionCacheComponent(ImmutableHashSet<int> idList) => _collidingWith = idList;

        public readonly ImmutableHashSet<int> CollidingWith => _collidingWith;

        public CollisionCacheComponent()
        {
        }

        public bool Contains<T>(World world) where T : IComponent
        {
            foreach (var id in _collidingWith)
            {
                if (world.TryGetEntity(id) is Entity entity && entity.HasComponent<T>())
                    return true;
            }

            return false;
        }
        public IEnumerable<Entity> GetCollidingEntities(World world)
        {
            foreach (var id in _collidingWith)
            {
                var entity = world.TryGetEntity(id);
                if (entity != null && !entity.IsDestroyed)
                    yield return entity;
            }
        }

        public bool HasId(int id) => _collidingWith.Contains(id);

        public CollisionCacheComponent Remove(int id)
        {
            return new(_collidingWith.Remove(id));
        }

        public CollisionCacheComponent Add(int id)
        {
            return new(_collidingWith.Add(id));
        }
    }
}