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
using System.Numerics;

namespace Murder.Systems
{
    /// <summary>
    /// System that looks for AgentImpulse systems and translates them into 'Velocity' for the physics system.
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

                // turn the character to face the direction of the impulse
                //  - unless the entity has strafing or
                //  - the entity has a facing turn component or
                //  - the impulse has no direction or
                //  - the impulse has the IgnoreFacing flag set
                if (!e.HasStrafing() &&
                    !e.HasFacingTurn() &&
                    impulse.Impulse != Vector2.Zero &&
                    impulse.Direction != null &&
                    !impulse.Flags.HasFlag(AgentImpulseFlags.IgnoreFacing))
                {
                    e.SetFacing(impulse.Impulse.Angle());
                }

                // do not change the velocity if there is no impulse to move
                if (!impulse.Impulse.HasValue())
                {
                    continue;
                }

                Vector2 startVelocity = e.TryGetVelocity()?.Velocity ?? Vector2.Zero;

                // calculate the new velocity, applying friction and multipliers
                Vector2 result = GetVelocity(e, agent, impulse, startVelocity);

                e.RemoveFriction();    // The friction component has been applied, so it can be removed
                e.SetVelocity(result); // apply the new velocity
            }
        }

        ///<summary>
        /// Calculates the new velocity of an entity based on its current velocity, the intended impulse and modifying components that apply.
        ///
        /// These include:
        /// - Friction from the AgentComponent
        /// - Speed multiplier from the AgentSpeedMultiplierComponent
        /// - Max speed and accelleration overrides from the AgentSpeedOverride component
        /// - Speed and slide multipliers from the InsideMovementModAreaComponent
        ///
        /// Velocity changes are scaled linearly, rather than applied immediately
        /// </summary>
        /// <param name="entity">The entity whose velocity is being calculated.</param>
        /// <param name="agent">The agent component of the entity.</param>
        /// <param name="impulse">The impulse (the intended direction of movement)</param>
        /// <param name="currentVelocity">The current velocity of the entity.</param>
        /// <returns>The new velocity of the entity.</returns>
        private static Vector2 GetVelocity(Entity entity, AgentComponent agent, AgentImpulseComponent impulse, in Vector2 currentVelocity)
        {
            var newVelocity = currentVelocity;

            // apply friction to make give the agent a weighty feeling if:
            // - the agent is not moving in the direction of the impulse
            // - the agent is not moving at all
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

            // apply a set of configurable, static speed multipliers from AgentSpeedMultiplierComponent
            //   - one for each of the eight directions
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

            // apply custom overrides for the agent's maximum speed and acceleration from AgentSpeedOverride component
            if (entity.TryGetOverrideAgentSpeed() is OverrideAgentSpeedComponent speedOverride)
            {
                speed = speedOverride.MaxSpeed;

                if (speedOverride.Acceleration != -1)
                {
                    accel = speedOverride.Acceleration;
                }
            }

            Vector2 finalImpulse = impulse.Impulse;

            // apply a speed multiplier and slide in either horizontal, vertical or both directions
            if (entity.TryGetInsideMovementModArea() is InsideMovementModAreaComponent areaList)
            {
                var normalized = impulse.Impulse.Normalized();

                if (areaList.GetLatest() is AreaInfo insideArea)
                {
                    // how much of the impulse direction is in the affected direction
                    float influence = OrientationHelper.GetOrientationAmount(normalized, insideArea.Orientation);

                    // apply the speed multiplier, scaled to the influence it has in that direction
                    multiplier *= Calculator.Lerp(1, insideArea.SpeedMultiplier, influence);

                    if (insideArea.Slide != 0)
                    {
                        // cast a vector perpendicular to the intended direction
                        //   - a positive slide will result in a push clockwise
                        //   - e.g. intent to move east will result in a push towards south, intent to move south will push towards west
                        //   - so if orientation is both directions, it will have a whirlpool effect
                        var perpendicular = insideArea.Slide < 0 ? finalImpulse.PerpendicularCounterClockwise() : finalImpulse.PerpendicularClockwise();

                        // apply the push on the intent according to how much it aligns with the slide orientation
                        finalImpulse = Vector2.Lerp(finalImpulse, perpendicular, influence * MathF.Abs(insideArea.Slide));
                    }
                }
            }

            // scale the velocity linearly towards the target velocity, rather than immediately jumping to the new velocity
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

            return EntityServices.IsMoving(e);
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