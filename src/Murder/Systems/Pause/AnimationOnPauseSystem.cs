using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Services;
using Murder.Utilities;

namespace Murder.Systems
{
    /// <summary>
    /// System that will automatically completes aseprites on a freeze cutscene.
    /// </summary>
    [Filter(typeof(FreezeWorldComponent))]
    public class AnimationOnPauseSystem : IUpdateSystem
    {
        public void Update(Context context)
        {
            if (context.Entities.Length == 0)
            {
                return;
            }

            if (!Game.Instance.IsSkippingDeltaTimeOnUpdate)
            {
                return;
            }

            foreach (Entity e in context.World.GetEntitiesWith(typeof(SpriteComponent)))
            {
                RenderServices.MessageCompleteAnimations(e);
                
                // some cutscenes depend on this to proceed
                e.SendAnimationEventMessage(string.Empty);
            }

            foreach (Entity e in context.World.GetEntitiesWith(typeof(AgentSpriteComponent)))
            {
                RenderServices.MessageCompleteAnimations(e);

                // some cutscenes depend on this to proceed
                e.SendAnimationEventMessage(string.Empty);
            }
        }
    }
}