using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Data;
using Murder.Utilities;
using Newtonsoft.Json;
using System.Collections.Immutable;

namespace Murder.Assets.Graphics;

public class SpriteAsset : GameAsset, IPreview
{
    [JsonProperty]
    public readonly AtlasId Atlas;

    [JsonProperty]
    public readonly ImmutableArray<AtlasCoordinates> Frames = ImmutableArray<AtlasCoordinates>.Empty;

    [JsonProperty]
    public ImmutableDictionary<string, Animation> Animations { get; private set; } = ImmutableDictionary<string, Animation>.Empty;

    [JsonProperty]
    public readonly Point Origin = new();

    [JsonProperty]
    public readonly Point Size;

    [JsonProperty]
    public readonly Rectangle NineSlice;

    public override char Icon => '\uf1fc';
    public override bool CanBeDeleted => false;
    public override bool CanBeRenamed => false;
    public override bool CanBeCreated => false;
    public override string EditorFolder => _editorPath;
    public override System.Numerics.Vector4 EditorColor => Game.Profile.Theme.Faded;

    private const string _prefixGeneratedPath = "#\uf085Generated";

    [JsonProperty]
    [HideInEditor]
    private string _editorPath = _prefixGeneratedPath;

    [JsonProperty]
    public AsepriteFileInfo? AsepriteFileInfo = null;

    [JsonConstructor]
    public SpriteAsset()
    { }

    public SpriteAsset(Guid guid, TextureAtlas atlas, string name, ImmutableArray<string> frames, ImmutableDictionary<string, Animation> animations, Point origin, Point size, Rectangle nineSlice)
    {
        Guid = guid;
        Name = name;
        Atlas = atlas.Id;
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

    public SpriteAsset(Guid guid, AtlasId atlasId, string name, ImmutableArray<string> frames, ImmutableDictionary<string, Animation> animations, Point origin, Point size, Rectangle nineSlice)
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

    public (AtlasId, string) GetPreviewId() => (Atlas, Frames[0].Name);

    public AtlasCoordinates GetFrame(int frame)
    {
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
}