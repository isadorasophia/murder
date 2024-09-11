using Bang.Components;
using Murder.Utilities;
using System.Collections.Immutable;
namespace Murder.Components;

public readonly struct SpriteFacingComponent : IComponent
{
    public readonly float AngleStart { get; init; } = 0; 
    public readonly string DefaultSuffix { get; init; } = string.Empty;
    public bool DefaultFlip { get; init; } = false;
    public readonly ImmutableArray<FacingInfo> FacingInfo { get; init; } = ImmutableArray<FacingInfo>.Empty;
    
    public SpriteFacingComponent() { }
    
    /// <summary>
    /// Returns a new <see cref="SpriteFacingComponent"/> with the <see cref="FacingInfo"/> resized.
    /// Trims the last items if 'slices' is smaller than current length, and adds default values if larger.
    /// </summary>
    public SpriteFacingComponent Resize(int atIndex, int slices)
    {
        if (slices <= FacingInfo.Length)
        {
            // If slices is less than or equal to the current length, trim the array
            var builder = ImmutableArray.CreateBuilder<FacingInfo>(slices);
            builder.AddRange(FacingInfo);

            for (int i = 0; i < FacingInfo.Length - slices; i++)
            {
                builder.RemoveAt(Math.Clamp(atIndex - 1, 0, FacingInfo.Length - 1));
            }

            return this with
            {
                FacingInfo = builder.ToImmutable()
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
                builder.Insert(Calculator.WrapAround(atIndex, 0, FacingInfo.Length), new FacingInfo() with { AngleSize = 0.1f }); // Assuming FacingInfo has a parameterless constructor
            }

            return this with
            {
                AngleStart = AngleStart,
                FacingInfo = builder.ToImmutable()
            };
        }
    }

    /// <summary>
    /// Gets the suffix and flip state based on a specified angle.
    /// </summary>
    /// <param name="angle">The angle in radians.</param>
    /// <returns>A tuple containing the suffix and flip state.</returns>
    public (string suffix, bool flip) GetSuffixFromAngle(float angle)
    {
        // Normalize the angle to be within the range [0, 2π)
        angle = (angle - AngleStart) % (2 * MathF.PI);

        float currentAngle = 0;
        foreach (var facingInfo in FacingInfo)
        {
            currentAngle += facingInfo.AngleSize;
            if (angle <= currentAngle)
            {
                return (facingInfo.Suffix, facingInfo.Flip);
            }
        }

        // If no segment found, return default suffix and flip
        return (DefaultSuffix, DefaultFlip);
    }
}

public readonly struct FacingInfo
{
    public readonly float AngleSize { get; init; }
    public readonly string Suffix { get; init; }
    public readonly bool Flip{ get; init; }
}
