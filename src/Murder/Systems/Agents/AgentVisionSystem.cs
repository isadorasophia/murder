using Bang.Contexts;
using Bang.Systems;
using Murder.Components;

namespace Murder.Systems
{
    // TODO: Could this be optimized to a reactive system? Not sure
    [Filter(typeof(PositionComponent), typeof(RequiresVisionComponent))]
    internal class AgentVisionSystem : IFixedUpdateSystem
    {
        public ValueTask FixedUpdate(Context context)
        {
            var map = context.World.GetUnique<MapComponent>().Map;

            foreach (var source in context.World.GetEntitiesWith(typeof(HasVisionComponent)))
            {
                // TODO: Generate extensions
                //var eye = source.GetGlobalPosition().CellPoint();
                //var sqMaxDistance = Math.Pow(source.GetHasVision().Range, 2);

                //foreach (var e in context.Entities)
                //{
                //    var target = e.GetGlobalPosition().CellPoint();
                //    var distanceSq = (target - eye).LengthSquared();
                //    if (distanceSq < sqMaxDistance)
                //    {
                //        if (map.HasLineOfSight(eye, target, false))
                //        {
                //            if (e.TryGetLastSeen() is LastSeenComponent lastSeen)
                //            {
                //                float lastTime = Math.Clamp(lastSeen.SeenTime, Time.Elapsed - 1, Time.Elapsed);
                //                float modifiedTime = Calculator.Approach(lastTime, Time.Elapsed, Game.FixedDeltaTime * 4);
                //                e.SetLastSeen(modifiedTime);

                //                if (modifiedTime == Time.Elapsed && e.GetRequiresVision().OnlyOnce)
                //                {
                //                    e.RemoveRequiresVision();
                //                }
                //            }
                //            else
                //            {
                //                if (e.GetRequiresVision().OnlyOnce)
                //                    e.SetLastSeen(Time.Elapsed - 3.5f);
                //                else
                //                    e.SetLastSeen(Time.Elapsed - 1);
                //            }
                //        }
                //    }
                //}
            }

            return default;
        }
    }
}
