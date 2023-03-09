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
        private readonly HashSet<int> _interactorId = new();

        public IsCollidingComponent(int id) => _interactorId = new() { id };
        public IsCollidingComponent(HashSet<int> idList) => _interactorId = idList;

        public bool Contains<T>(World world) where T : IComponent
        {
            foreach (var id in _interactorId)
            {
                if (world.TryGetEntity(id) is Entity entity && entity.HasComponent<T>())
                    return true;
            }

            return false;
        }

        public bool HasId(int id) => _interactorId.Contains(id);
        
        public IsCollidingComponent Remove(int id)
        {
            _interactorId.Remove(id);
            return new(_interactorId);
        }
        public IsCollidingComponent Add(int id)
        {
            _interactorId.Add(id);
            return new(_interactorId);
        }
    }
}
