using Bang.Components;
using Murder.Core;

namespace Murder.Components.Utilities;

/// <summary>
/// Currently used for some physics check, 
/// e.g. see <see cref="Murder.Systems.AgentMovementModifierSystem"/>.
/// </summary>
public readonly struct TagsComponent : IComponent
{
    public readonly Tags Tags;
}
