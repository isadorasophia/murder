using Bang.Components;
using System.Collections.Immutable;
namespace Murder.Components;

public readonly struct SpriteFacingComponent : IComponent
{

    public readonly float AngleStart { get; init; } = 0; 
    public readonly string DefaultSuffix { get; init; } = string.Empty;
    public bool DefaultFlip { get; init; } = false;
    public readonly ImmutableArray<FacingInfo> FacingInfo { get; init; } = ImmutableArray<FacingInfo>.Empty;
    
    public SpriteFacingComponent()
    {
        
    }
    
    /// <summary>
    /// Returns a new <see cref="SpriteFacingComponent"/> with the <see cref="FacingInfo"/> resized.
    /// Trims the last items if 'slices' is smaller than current length, and adds default values if larger.
    /// </summary>
    /// <param name="slices">The new length of the FacingInfo array.</param>
    /// <returns>A new SpriteFacingComponent with resized FacingInfo.</returns>
    public SpriteFacingComponent Resize(int slices)
    {
        if (slices <= FacingInfo.Length)
        {
            // If slices is less than or equal to the current length, trim the array
            return new SpriteFacingComponent() with
            {
                AngleStart = AngleStart,
                FacingInfo = FacingInfo.Slice(0, slices)
            };
        }
        else
        {
            // If slices is greater than the current length, extend the array
            var builder = ImmutableArray.CreateBuilder<FacingInfo>(slices);
            builder.AddRange(FacingInfo);

            // Adding default or empty FacingInfo objects to fill the remaining space
            for (int i = FacingInfo.Length; i < slices; i++)
            {
                builder.Add(new FacingInfo()); // Assuming FacingInfo has a parameterless constructor
            }

            return new SpriteFacingComponent() with
            {
                AngleStart = AngleStart,
                FacingInfo = builder.ToImmutable()
            };
        }
    }
}
public readonly struct FacingInfo
{
    public readonly float AngleSize { get; init; }
    public readonly string Suffix { get; init; }
    public readonly bool Flip{ get; init; }
}
