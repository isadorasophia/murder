using Bang.Components;
using Bang.Contexts;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Graphics;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Core;
using Murder.Editor.Services;

namespace Murder.Editor.Systems.Sounds
{
    [SoundEditor]
    [PrefabEditor]
    [OnlyShowOnDebugView]
    [Filter(typeof(ColliderComponent), typeof(ITransformComponent))]
    [Filter(ContextAccessorFilter.AnyOf, typeof(SoundComponent), typeof(SoundParameterComponent))]
    public class SoundColliderEditorSystem : IUpdateSystem, IMurderRenderSystem
    {
        private bool _wasClicking = false;

        public void Update(Context context)
        {
            if (EditorServices.LastDragStyle == EditorServices.DragStyle.Move)
            {
                if (context.World.TryGetUnique<EditorComponent>() is EditorComponent editor)
                {
                    editor.EditorHook.Cursor = CursorStyle.Hand;
                }
            }
        }

        public void Draw(RenderContext render, Context context)
        {
            DebugColliderRenderSystem.DrawImpl(render, context, allowEditingByDefault: false, ref _wasClicking);
        }
    }
}