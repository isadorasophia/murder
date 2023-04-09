using Gum;
using Gum.InnerThoughts;
using Murder.Assets;
using Murder.Diagnostics;
using Murder.Editor.Utilities;
using Murder.Serialization;
using System.Collections.Immutable;

namespace Murder.Editor.Data
{
    public partial class EditorDataManager
    {
        private const string _dialogsDescriptorName = "descriptor.json";

        private GumToMurderConverter? _dialogConverter = null;

        /// <summary>
        /// This will load all the sounds to the game.
        /// </summary>
        public ValueTask ReloadDialogs(bool force = false)
        {
            if (!Directory.Exists(EditorSettings.RawResourcesPath))
            {
                GameLogger.Log($"Unable to find raw resources path at {FileHelper.GetPath(EditorSettings.RawResourcesPath)}. " +
                    $"Use this directory for loading dialog assets.");

                return default;
            }

            string dialogsRawResourcesPath = FileHelper.GetPath(Path.Join(EditorSettings.RawResourcesPath, GameProfile.DialogsPath));
            if (!Directory.Exists(dialogsRawResourcesPath))
            {
                // No dialogs found, just go away...?
                return default;
            }

            string dialogsPackedPath = FileHelper.GetPath(Path.Join(EditorSettings.SourcePackedPath, GameProfile.DialogsPath));

            string descriptorPath = Path.Join(dialogsPackedPath, _dialogsDescriptorName);
            if (force || !FileLoadHelpers.ShouldRecalculate(dialogsRawResourcesPath, descriptorPath))
            {
                GameLogger.Log("Skipping refreshing dialogs because everything seems up to date.");
                return default;
            }

            GameLogger.Log("Starting to convert *.gum dialogs...");

            DateTime? target = force ? null : File.GetLastWriteTime(descriptorPath);
            ProcessDialogs(dialogsRawResourcesPath, target);

            GameLogger.Log("Finished generating dialogs!");

            // Create descriptor file to refresh the cache.
            File.Create(descriptorPath);

            return default;
        }
        
        /// <param name="path">Target file of the *.gum files.</param>
        /// <param name="lastModified">If null, disregard and apply to all the files.</param>
        private void ProcessDialogs(string path, DateTime? lastModified)
        {
            ImmutableDictionary<string, Guid> characterAssets = FindAllNamesForAssetWithGuid(typeof(CharacterAsset));

            if (_dialogConverter is null)
            {
                _dialogConverter = new();
            }
            else
            {
                _dialogConverter.Reset();
            }

            CharacterScript[] scripts = Reader.Parse(path, lastModified, out string errors);
            foreach (CharacterScript script in scripts)
            {
                CharacterAsset asset;
                if (characterAssets.TryGetValue(script.Name, out Guid id))
                {
                    asset = (CharacterAsset)_allAssets[id];
                }
                else
                {
                    asset = new();
                    asset.Name = Prettify.CapitalizeFirstLetter(script.Name);

                    AddAsset(asset);
                }

                _dialogConverter.ReloadDialogWith(script, asset);
                SaveAsset(asset);
            }
        }

        /// <summary>
        /// Find all the assets names for an asset type <paramref name="t"/>.
        /// </summary>
        /// <param name="t">The type that inherist from <see cref="GameAsset"/>.</param>
        public ImmutableDictionary<string, Guid> FindAllNamesForAssetWithGuid(Type t)
        {
            ImmutableDictionary<string, Guid> result = ImmutableDictionary<string, Guid>.Empty;

            if (_database.TryGetValue(t, out HashSet<Guid>? assetGuids))
            {
                result = assetGuids.ToDictionary(g => Path.GetFileNameWithoutExtension(_allAssets[g].Name), g => g)
                    .ToImmutableDictionary(StringComparer.OrdinalIgnoreCase);
            }

            return result;
        }
    }
}