using Murder.Utilities.Attributes;
using System.ComponentModel;

namespace Murder.Data;

// Description must be atlas file name
public enum AtlasId
{
    None,
    [Description(AtlasIdentifiers.Gameplay)]
    Gameplay,
    [Description(AtlasIdentifiers.Editor)]
    Editor,
    [Description(AtlasIdentifiers.Static)]
    Static,
    [Description(AtlasIdentifiers.Temporary)]
    Temporary,
    [Description(AtlasIdentifiers.Preload)]
    Preload,
    [Description(AtlasIdentifiers.Cutscenes)]
    Cutscenes,
    //[Description("main_menu")]
    //MainMenu,
    //[Description("portraits")]
    //Portraits,
}

public static class AtlasIdentifiers
{
    public const string Gameplay = "atlas";
    public const string Editor = "editor";
    public const string Static = "static";
    public const string Temporary = "temporary";
    public const string Preload = "preload";
    public const string Cutscenes = "cutscenes";
}

public readonly struct ReferencedAtlas
{
    [AtlasName]
    public readonly string Id = string.Empty;

    public readonly bool UnloadOnExit = false;

    public ReferencedAtlas() { }

    public ReferencedAtlas(string id, bool unloadOnExit) =>
        (Id, UnloadOnExit) = (id, unloadOnExit);
}