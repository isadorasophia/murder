using Bang;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Data;
using Murder.Diagnostics;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Assets.Graphics;

[SkipJsonFileIntegrityCheck]
public class SpriteAsset : GameAsset, IPreview
{
    [Serialize]
    public readonly string Atlas = string.Empty;

    [Serialize]
    public ImmutableArray<AtlasCoordinates> Frames { get; private set; } = ImmutableArray<AtlasCoordinates>.Empty;

    [Serialize]
    public ImmutableDictionary<string, Animation> Animations { get; private set; } = ImmutableDictionary<string, Animation>.Empty;

    [Serialize]
    public readonly Point Origin = new();

    [Serialize]
    public readonly Point Size;

    [Serialize]
    public readonly Rectangle NineSlice;

    public override char Icon => '\uf1fc';
    public override bool CanBeDeleted => false;
    public override bool CanBeRenamed => false;
    public override bool CanBeCreated => false;
    public override string EditorFolder => _editorPath;
    public override System.Numerics.Vector4 EditorColor => Game.Profile.Theme.Faded;

    private const string _prefixGeneratedPath = "#\uf085Generated";

    [Serialize]
    [HideInEditor]
    private string _editorPath = _prefixGeneratedPath;

    public SpriteAsset() { }

    public SpriteAsset(Guid guid, TextureAtlas atlas, string name, ImmutableArray<string> frames, ImmutableDictionary<string, Animation> animations, Point origin, Point size, Rectangle nineSlice)
    {
        Guid = guid;
        Name = name;
        Atlas = atlas.AtlasId;
        Animations = animations;
        Origin = origin;
        NineSlice = nineSlice;

        Size = size;

        var builder = ImmutableArray.CreateBuilder<AtlasCoordinates>(frames.Length);
        foreach (var frame in frames)
        {
            var coord = atlas.Get(frame);
            builder.Add(coord);
        }
        Frames = builder.ToImmutable();
    }

    public SpriteAsset(Guid guid, string atlasId, string name, ImmutableArray<string> frames, ImmutableDictionary<string, Animation> animations, Point origin, Point size, Rectangle nineSlice)
    {
        Guid = guid;
        Name = name;
        Atlas = atlasId;
        Animations = animations;
        Origin = origin;
        NineSlice = nineSlice;

        var atlas = Game.Data.FetchAtlas(atlasId);
        Size = size;

        var builder = ImmutableArray.CreateBuilder<AtlasCoordinates>(frames.Length);
        foreach (var frame in frames)
        {
            var coord = atlas.Get(frame);
            builder.Add(coord);
        }
        Frames = builder.ToImmutable();
    }

    public (string, string) GetPreviewId() => (Atlas, Frames[0].Name);

    public AtlasCoordinates GetFrame(int frame)
    {
        if (frame < 0 || frame >= Frames.Length)
        {
            GameLogger.Error($"Trying to get frame {frame} from sprite {Name} which only has {Frames.Length} frames. Returning empty coordinates.");
            return AtlasCoordinates.Empty;
        }
        return Frames[frame];
    }

    /// <summary>
    /// Set a directory prefix used for the editor folder.
    /// </summary>
    public void AppendEditorPath(string prefix)
    {
        _editorPath = Path.Join(_prefixGeneratedPath, prefix);
    }

    public bool AddMessageToAnimationFrame(string animationName, int frame, string message)
    {
        if (!Animations.TryGetValue(animationName, out Animation animation))
        {
            return false;
        }

        if (animation.Events is null)
        {
            return false;
        }

        // Check if the message was already added.
        if (animation.Events.TryGetValue(frame, out string? previousMessage) && message == previousMessage)
        {
            return false;
        }

        animation = animation.WithMessageAt(frame, message);

        // Update animations with the new value.
        Animations = Animations.SetItem(animationName, animation);
        FileChanged = true;

        return true;
    }

    public bool RemoveMessageFromAnimationFrame(string animationName, int frame)
    {
        if (!Animations.TryGetValue(animationName, out Animation animation))
        {
            return false;
        }

        if (animation.Events is null)
        {
            return false;
        }

        if (!animation.Events.ContainsKey(frame))
        {
            return false;
        }

        animation = animation.WithoutMessageAt(frame);

        // Update animations with the new value.
        Animations = Animations.SetItem(animationName, animation);
        FileChanged = true;

        return true;
    }

    /// <summary>
    /// This will merge a sprite asset with an existing one.
    /// </summary>
    public void MergeWith(SpriteAsset other)
    {
        int frameOffset = Frames.Length;

        Frames = Frames.AddRange(other.Frames);

        // iterate other's animations, decide what to do per key.
        var builder = Animations.ToBuilder();
        foreach ((string name, Animation anim) in other.Animations)
        {
            Animation remapped = ShiftAnimationFrames(anim, frameOffset);

            if (string.IsNullOrEmpty(name))
            {
                // We ignore the empty name animation.
                // Concatenated sprites don't care about those since we couldn't deal with the order of frames.
                continue;
            }

            // Named animation: error on collision, keep first.
            if (builder.ContainsKey(name))
            {
                GameLogger.Warning(
                    $"Merge conflict on sprite '{Name}' (GUID {Guid}): animation '{name}' " +
                    $"exists in multiple source files. Keeping the first. " +
                    $"Existing source: {this.Name}, " +
                    $"Rejected source: {other.Name}");

                continue;
            }

            builder[name] = remapped;
        }

        Animations = builder.ToImmutable();

        // keep ours, warn on drift.
        // different names are fine
        WarnIfDifferent(nameof(Origin), Origin, other.Origin);
        WarnIfDifferent(nameof(Size), Size, other.Size);
        WarnIfDifferent(nameof(NineSlice), NineSlice, other.NineSlice);
    }

    public void WarnIfDifferent(string whatsDifferent, Rectangle origin1, Rectangle origin2)
    {
        if (origin1 != origin2)
        {
            GameLogger.Warning($"Sprite '{Name}' (GUID {Guid}): {whatsDifferent} differs between sources. " +
                               $"Existing: {origin1}, New: {origin2}");
        }
    }

    private void WarnIfDifferent(string whatsDifferent, Point origin1, Point origin2)
    {
        if (origin1 != origin2)
        {
            GameLogger.Warning($"Sprite '{Name}' (GUID {Guid}): {whatsDifferent} differs between sources. " +
                               $"Existing: {origin1}, New: {origin2}");
        }
    }

    /// <summary>
    /// Shifts an animation's Frames values by the given offset so they index
    /// /// </summary>
    private static Animation ShiftAnimationFrames(Animation anim, int globalFrameOffset)
    {
        var framesBuilder = ImmutableArray.CreateBuilder<int>(anim.Frames.Length);
        for (int i = 0; i < anim.Frames.Length; i++)
        {
            framesBuilder.Add(anim.Frames[i] + globalFrameOffset);
        }

        return new Animation(
            framesBuilder.ToImmutable(),
            anim.FramesDuration,
            anim.Events,
            anim.AnimationDuration,
            anim.NextAnimation);
    }

}