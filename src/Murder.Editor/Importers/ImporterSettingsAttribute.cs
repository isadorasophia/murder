namespace Murder.Editor.Importers;

public enum FilterType
{
    All,
    OnlyTheseFolders,
    ExceptTheseFolders,
    /// <summary>
    /// This will ignore any filters to be applied. This is usually when another
    /// importer delegate the files manually to it.
    /// </summary>
    Ignore,
    None
}

[AttributeUsage(AttributeTargets.Class)]
public class ImporterSettingsAttribute : Attribute
{
    internal readonly FilterType FilterType;
    internal readonly string[] FilterFolders = [];
    internal readonly string[] FileExtensions = [];

    /// <summary>
    /// Whether this should be immediately loaded on start.
    /// This is usually progress images which will be loaded while the games continues the loading operations.
    /// </summary>
    internal bool LoadOnStart = false;

    public ImporterSettingsAttribute(FilterType filterType, string[] filterFolders, string[] fileExtensions, bool loadOnStart = false)
    {
        FilterType = filterType;
        FilterFolders = filterFolders;
        FileExtensions = fileExtensions;
        LoadOnStart = loadOnStart;
    }

    public ImporterSettingsAttribute(params string[] fileExtensions)
    {
        FilterType = FilterType.All;
        FilterFolders = [];
        FileExtensions = fileExtensions;
    }

    public ImporterSettingsAttribute(FilterType filterType)
    {
        FilterType = filterType;
    }
}