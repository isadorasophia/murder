using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Graphics;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Utilities;
using Murder.Services;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Editor.Systems.Debug;

[OnlyShowOnDebugView]
[Filter(ContextAccessorFilter.AllOf, typeof(VelocityComponent), typeof(PositionComponent))]
public class PhysicsDebugSystem : IMurderRenderSystem
{
    public void Draw(RenderContext render, Context context)
    {
        // TODO: Move this to a debug system.
        EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;

        foreach (var e in context.Entities)
        {
            Vector2 position = e.GetGlobalTransform().Vector2;
            Vector2 velocity = e.GetVelocity().Velocity;
            Color color = Color.Lerp(Color.Green, Color.Red, Calculator.Clamp01(velocity.Length() / 1000f));

            RenderServices.DrawArrow(render.DebugBatch, position, position + velocity * 0.5f, color, 1, 3, 1);
        }
    }
}
