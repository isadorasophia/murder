using Bang;
using Bang.Components;
using Bang.Entities;
using Murder.Attributes;
using System.Collections.Immutable;

namespace Murder.Components
{
    public readonly struct IsCollidingComponent : IComponent
    { 
        /// <summary>
        /// Id of the entity that caused this collision.
        /// </summary>
        [ShowInEditor]
        private readonly HashSet<int> _collindingWith = new();

        public IsCollidingComponent(int id) => _collindingWith = new() { id };
        public IsCollidingComponent(HashSet<int> idList) => _collindingWith = idList;

        public bool Contains<T>(World world) where T : IComponent
        {
            foreach (var id in _collindingWith)
            {
                if (world.TryGetEntity(id) is Entity entity && entity.HasComponent<T>())
                    return true;
            }

            return false;
        }
        public IEnumerable<Entity> GetCollidingEntities(World world)
        {
            foreach (var id in _collindingWith)
            {
                var entity = world.TryGetEntity(id);
                if (entity!=null && !entity.IsDestroyed)
                    yield return entity;
            }
        }

        public bool HasId(int id) => _collindingWith.Contains(id);
        
        public IsCollidingComponent Remove(int id)
        {
            _collindingWith.Remove(id);
            return new(_collindingWith);
        }
        public IsCollidingComponent Add(int id)
        {
            _collindingWith.Add(id);
            return new(_collindingWith);
        }
    }
}
