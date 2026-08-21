using Bang.Components;
using Bang.Contexts;
using Bang.Systems;
using Murder.Components;
using Murder.Editor.Attributes;

namespace Murder.Editor.Systems;

[SoundEditor]
[PrefabEditor]
[OnlyShowOnDebugView]
[Filter(typeof(ColliderComponent), typeof(PositionComponent))]
[Filter(ContextAccessorFilter.AnyOf, typeof(SoundComponent), typeof(SoundParameterComponent))]
public class SoundColliderEditorSystem : BaseDebugColliderRenderSystem
{
}