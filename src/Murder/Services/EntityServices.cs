using Bang;
using Bang.Entities;
using Microsoft.Xna.Framework;

namespace Murder.Services
{
    public static class EntityServices
    {
        public static void RotateChildPositions(World world, Entity entity, float angle)
        {
            foreach (var id in entity.Children)
            {
                if (world.TryGetEntity(id) is Entity child)
                {
                    var localPosition = child.GetPosition().Pos;
                    child.SetPosition(localPosition.Rotate(angle));
                }
            }
        }
        public static void RotatePositionAround(Entity entity, Vector2 center, float angle)
        {
            var localPosition = entity.GetPosition().Pos - center;
            entity.SetPosition(center + localPosition.Rotate(angle));
        }
        public static void RotatePosition(Entity entity, float angle)
        {
            var localPosition = entity.GetPosition().Pos;
            entity.SetPosition(localPosition.Rotate(angle));
        }

        public static bool IsChildOf(World world, Entity parent, Entity child)
        {
            if (child.Parent.HasValue)
            {
                if (child.Parent.Value == parent.EntityId)
                {
                    return true;
                }
                else
                {
                    if (world.TryGetEntity(child.Parent.Value) is Entity parentEntity)
                    {
                        return IsChildOf(world, parent, parentEntity);
                    }
                }
            }

            return false;
        }
    }
}
