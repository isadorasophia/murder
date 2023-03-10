using Bang;
using Bang.Components;
using Bang.Entities;
using Murder.Attributes;
using Newtonsoft.Json;
using System.Collections.Immutable;

namespace Murder.Components
{
    public readonly struct CollisionCacheComponent : IComponent
    { 
        /// <summary>
        /// Id of the entity that caused this collision.
        /// </summary>
        private readonly HashSet<int> _collidingWith = new();

        public CollisionCacheComponent(int id) => _collidingWith = new() { id };
        public CollisionCacheComponent(HashSet<int> idList) => _collidingWith = idList;

        public CollisionCacheComponent()
        {
            _collidingWith = new();
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
                if (entity!=null && !entity.IsDestroyed)
                    yield return entity;
            }
        }

        public bool HasId(int id) => _collidingWith.Contains(id);
        
        public CollisionCacheComponent Remove(int id)
        {
            _collidingWith.Remove(id);
            return new(_collidingWith);
        }
        public CollisionCacheComponent Add(int id)
        {
            _collidingWith.Add(id);
            return new(_collidingWith);
        }
    }
}
