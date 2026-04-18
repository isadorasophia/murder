using Bang.Components;

namespace Murder.Components.Agents;

public readonly struct InsideGravityWellComponent : IComponent
{
    public readonly int GravityWellId = -1;

    public InsideGravityWellComponent(int id)
    {
        GravityWellId = id;
    }
}
