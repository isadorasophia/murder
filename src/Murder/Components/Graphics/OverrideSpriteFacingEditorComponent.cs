using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Components;

public readonly struct SpriteFacingId
{
    public readonly string Id = string.Empty;

    public readonly SpriteFacingComponent Info = new();

    public SpriteFacingId() { }
}

public readonly struct OverrideSpriteFacingEditorComponent : IComponent
{
    public readonly ImmutableArray<SpriteFacingId> SpriteFaces = [];

    public OverrideSpriteFacingEditorComponent() { }
}

[RuntimeOnly]
[PersistOnSave]
[HideInEditor]
public readonly struct OverrideSpriteFacingComponent : IComponent
{
    public readonly ImmutableDictionary<string, SpriteFacingComponent> Faces = 
        ImmutableDictionary<string, SpriteFacingComponent>.Empty;

    public OverrideSpriteFacingComponent() { }

    public OverrideSpriteFacingComponent(ImmutableDictionary<string, SpriteFacingComponent> faces) =>
        Faces = faces;
}