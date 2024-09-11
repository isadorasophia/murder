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
public readonly struct OverrideSpriteFacingComponent
{
    public readonly ImmutableDictionary<string, SpriteFacingId> Faces = 
        ImmutableDictionary<string, SpriteFacingId>.Empty;

    public OverrideSpriteFacingComponent() { }

    public OverrideSpriteFacingComponent(ImmutableDictionary<string, SpriteFacingId> faces) =>
        Faces = faces;
}