using Bang.Components;
using Murder.Attributes;
using Newtonsoft.Json;

namespace Murder.Components;

/// <summary>
/// For now, this is only supported for aseprite components.
/// </summary>
public readonly struct FadeWhenInCutsceneComponent : IComponent
{
    public readonly float Duration = 0;

    [HideInEditor]
    [JsonIgnore]
    public readonly float PreviousAlpha = 0;

    public FadeWhenInCutsceneComponent() { }

    public FadeWhenInCutsceneComponent(float duration, float previousAlpha) =>
        (Duration, PreviousAlpha) = (duration, previousAlpha);

    public FadeWhenInCutsceneComponent TrackAlpha(float alpha) => new(Duration, alpha);
}
