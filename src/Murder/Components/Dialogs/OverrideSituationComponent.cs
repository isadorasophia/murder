using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Components;

public enum SituationOrigin
{
    Any = 0,
    Scene = 1,
    Moving = 2,
    Story = 3
}

[RuntimeOnly]
[PersistOnSave]
public readonly struct OverrideSituationComponent : IComponent
{
    private readonly static SituationOrigin[] _origins = 
        [SituationOrigin.Story, SituationOrigin.Moving, SituationOrigin.Scene, SituationOrigin.Any];

    public readonly ImmutableDictionary<SituationOrigin, SituationComponent> Situations = 
        ImmutableDictionary<SituationOrigin, SituationComponent>.Empty;

    public OverrideSituationComponent() { }

    public OverrideSituationComponent(ImmutableDictionary<SituationOrigin, SituationComponent> situations)
    {
        Situations = situations;
    }

    public OverrideSituationComponent WithSituation(SituationOrigin origin, SituationComponent situation)
    {
        return new(Situations.SetItem(origin, situation));
    }

    public OverrideSituationComponent WithoutSituation(SituationOrigin origin)
    {
        return new(Situations.Remove(origin));
    }

    public bool IsEmpty => Situations.IsEmpty;

    public SituationComponent? Peek
    {
        get
        {
            foreach (SituationOrigin origin in _origins)
            {
                if (Situations.TryGetValue(origin, out SituationComponent result))
                {
                    return result;
                }
            }

            return null;
        }
    }
}
