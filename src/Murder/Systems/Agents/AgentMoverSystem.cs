using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Components.Agents;
using Murder.Components.Sound;
using Murder.Core;
using Murder.Core.Sounds;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Numerics;

namespace Murder.Systems
{
    /// <summary>
    /// System that looks for AgentImpulse systems and translated them into 'Velocity' for the physics system.
    /// </summary>
    [Filter(typeof(AgentComponent), typeof(AgentImpulseComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(DisableAgentComponent), typeof(AgentPauseComponent))]
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

                if (!e.HasStrafing() &&
                    !e.HasFacingTurn() &&
                    impulse.Direction != null &&
                    !impulse.Flags.HasFlag(AgentImpulseFlags.IgnoreFacing))
                {
                    e.SetFacing(impulse.Impulse.Angle());
                }

                if (!impulse.Impulse.HasValue())
                {
                    continue;
                }

                Vector2 startVelocity = e.TryGetVelocity()?.Velocity ?? Vector2.Zero;

                // Use friction on any axis that's not receiving impulse or is receiving it in an oposing direction
                Vector2 result = GetVelocity(e, agent, impulse, startVelocity);

                e.RemoveFriction();     // Remove friction to move
                e.SetVelocity(result); // Turn impulse into velocity
            }
        }

        private static Vector2 GetVelocity(Entity entity, AgentComponent agent, AgentImpulseComponent impulse, in Vector2 currentVelocity)
        {
            var newVelocity = currentVelocity;

            if (impulse.Impulse.HasValue())
            {
                if (impulse.Impulse.X == 0 || !Calculator.SameSignOrSimilar(impulse.Impulse.X, currentVelocity.X))
                {
                    newVelocity = new Vector2(currentVelocity.X * agent.Friction, newVelocity.Y);
                }
                if (impulse.Impulse.Y == 0 || !Calculator.SameSignOrSimilar(impulse.Impulse.Y, currentVelocity.Y))
                {
                    newVelocity = new Vector2(newVelocity.X, newVelocity.Y * agent.Friction);
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

            float speed = agent.Speed;
            float accel = agent.Acceleration;

            if (entity.TryGetOverrideAgentSpeed() is OverrideAgentSpeedComponent speedOverride)
            {
                speed = speedOverride.MaxSpeed;

                if (speedOverride.Acceleration != -1)
                {
                    accel = speedOverride.Acceleration;
                }
            }

            Vector2 finalImpulse = impulse.Impulse;

            if (entity.TryGetInsideMovementModArea() is InsideMovementModAreaComponent areaList)
            {
                var normalized = impulse.Impulse.Normalized();

                if (areaList.GetLatest() is AreaInfo insideArea)
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


            return Calculator.Approach(newVelocity, finalImpulse * speed * multiplier, accel * multiplier * Game.FixedDeltaTime);
        }
    }

    [Filter(typeof(AgentComponent), typeof(AgentOnMoveTrackerComponent))]
    [Watch(typeof(AgentOnMoveTrackerComponent))]
    internal class AgentMoverSoundSystem : IFixedUpdateSystem, IReactiveSystem
    {
        public void OnAdded(World world, ImmutableArray<Entity> entities) { }

        public void OnModified(World world, ImmutableArray<Entity> entities) { }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            foreach (Entity e in entities)
            {
                if (e.TryGetSoundEventPositionTracker() is SoundEventPositionTrackerComponent tracker)
                {
                    Stop(tracker.Sound, e);
                }
            }
        }

        public void OnDeactivate(World world, ImmutableArray<Entity> entities)
        {
            OnRemoved(world, entities);
        }

        public void FixedUpdate(Context context)
        {
            foreach (Entity e in context.Entities)
            {
                AgentOnMoveTrackerComponent onMoveTracker = e.GetAgentOnMoveTracker();
                if (IsMoving(e))
                {
                    Play(onMoveTracker.OnWalk, e);
                }
                else
                {
                    Stop(onMoveTracker.OnWalk, e);
                }
            }
        }

        private bool IsMoving(Entity e)
        {
            if (e.HasAgentPause())
            {
                return false;
            }

            return e.HasVelocity() || e.HasAgentImpulse() || e.HasMoveToPerfect();
        }

        private void Play(SoundEventId sound, Entity e)
        {
            if (SoundServices.IsPlaying(sound, e.EntityId))
            {
                return;
            }

            _ = SoundServices.Play(sound, e, SoundLayer.Sfx, SoundProperties.Persist);
            e.SetSoundEventPositionTracker(sound);
        }

        private void Stop(SoundEventId sound, Entity e)
        {
            if (!SoundServices.IsPlaying(sound, e.EntityId))
            {
                return;
            }

            SoundServices.Stop(sound, fadeOut: true, e.EntityId);
            e.RemoveSoundEventPositionTracker();
        }
    }
}