﻿using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Components.Agents;
using Murder.Core;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Systems
{
    /// <summary>
    /// System that looks for AgentImpulse systems and translated them into 'Velocity' for the physics system.
    /// </summary>
    [Filter(typeof(AgentComponent), typeof(AgentImpulseComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(DisableAgentComponent))]
    internal class AgentMoverSystem : IUpdateSystem
    {
        public void Update(Context context)
        {
            ImpulseToVelocity(context.Entities);
        }

        public void ImpulseToVelocity(ImmutableArray<Entity> entities)
        {
            foreach (var e in entities)
            {
                var agent = e.GetAgent();
                var impulse = e.GetAgentImpulse();

                if (!e.HasStrafing() && impulse.Direction != null)
                    e.SetFacing(impulse.Impulse.Angle());

                if (!impulse.Impulse.HasValue())
                    continue;

                Vector2 startVelocity = e.TryGetVelocity()?.Velocity ?? Vector2.Zero;

                // Use friction on any axis that's not receiving impulse or is receiving it in an oposing direction
                var result = GetVelocity(e, agent, impulse, startVelocity);

                e.RemoveFriction();     // Remove friction to move
                e.SetVelocity(result); // Turn impulse into velocity
            }
        }

        private static Vector2 GetVelocity(Entity entity, AgentComponent agent, AgentImpulseComponent impulse, in Vector2 currentVelocity)
        {
            var velocity = currentVelocity;

            if (impulse.Impulse.HasValue())
            {
                if (impulse.Impulse.X == 0 || !Calculator.SameSignOrSimilar(impulse.Impulse.X, currentVelocity.X))
                {
                    velocity = new Vector2(currentVelocity.X * agent.Friction, velocity.Y);
                }
                if (impulse.Impulse.Y == 0 || !Calculator.SameSignOrSimilar(impulse.Impulse.Y, currentVelocity.Y))
                {
                    velocity = new Vector2(velocity.X, velocity.Y * agent.Friction);
                }
            }

            float multiplier = 1f;
            if (entity.TryGetAgentSpeedMultiplier() is AgentSpeedMultiplierComponent speedMultiplier)
            {
                for (int i = 0; i < speedMultiplier.SpeedMultiplier.Length; i++)
                {
                    multiplier *= speedMultiplier.SpeedMultiplier[i];
                }
            }

            float speed, accel;
            if (entity.TryGetAgentSpeedOverride() is AgentSpeedOverride speedOverride)
            {
                speed = speedOverride.MaxSpeed;
                accel = speedOverride.Acceleration;
            }
            else
            {
                speed = agent.Speed;
                accel = agent.Acceleration;
            }

            Vector2 finalImpulse = impulse.Impulse;

            if (entity.TryGetInsideMovementModArea() is InsideMovementModAreaComponent areaList)
            {
                var normalized = impulse.Impulse.Normalized();
                
                if (areaList.GetLatest() is InsideMovementModAreaComponent.AreaInfo insideArea)
                {
                    float influence = OrientationHelper.GetOrientationAmount(normalized, insideArea.Orientation);

                    multiplier *= Calculator.Lerp(1, insideArea.SpeedMultiplier, influence);

                    if (insideArea.Slide != 0)
                    {
                        var perpendicular = insideArea.Slide < 0 ? finalImpulse.PerpendicularCounterClockwise() : finalImpulse.PerpendicularClockwise();
                        finalImpulse = Vector2.Lerp(finalImpulse, perpendicular, influence * MathF.Abs(insideArea.Slide));
                    }
                }
            }

            return Calculator.Approach(velocity, finalImpulse * speed * multiplier, accel * multiplier * Game.FixedDeltaTime);
        }
    }
}