using Murder.Serialization;

namespace Murder.Data;

public partial class GameDataManager
{
    /// <summary>
    /// Relative path where the published game content are expected to be.
    /// Expected to be:
    ///     [bin]/resources/content/
    ///     or
    ///     [source]/packed/content/
    /// </summary>
    protected const string _packedGameDataDirectory = "content";

    protected const string _packedGameDataFilename = "data.gz";

    /// <summary>
    /// File path of the packed contents for the released game.
    /// </summary>
    public virtual string PublishedPackedAssetsFullPath => FileHelper.GetPath(Game.Data.BinResourcesDirectoryPath, _packedGameDataDirectory);
}
