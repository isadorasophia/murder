using Gum;
using Gum.InnerThoughts;
using Murder.Assets;
using Murder.Diagnostics;
using Murder.Editor.Utilities;
using Murder.Serialization;
using Murder.Utilities;
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
        public bool ReloadDialogs(bool force = false)
        {
            string fullRawResourcesPath = FileHelper.GetPath(EditorSettings.RawResourcesPath);
            if (!Directory.Exists(fullRawResourcesPath))
            {
                GameLogger.Log($"Unable to find raw resources path at {fullRawResourcesPath}. " +
                    $"Use this directory for loading dialog assets.");

                return false;
            }

            string dialogsRawResourcesPath = Path.Join(fullRawResourcesPath, GameProfile.DialoguesPath);
            if (!Directory.Exists(dialogsRawResourcesPath))
            {
                // No dialogs found, just go away...?
                return false;
            }

            string dialogsPackedPath = FileHelper.GetPath(Path.Join(EditorSettings.SourcePackedPath, GameProfile.DialoguesPath));

            string descriptorPath = Path.Join(dialogsPackedPath, _dialogsDescriptorName);
            if (force || !FileLoadHelpers.ShouldRecalculate(dialogsRawResourcesPath, descriptorPath))
            {
                return false;
            }

            GameLogger.Log("Starting to convert *.gum dialogs...");

            DateTime? target = force ? null : File.GetLastWriteTime(descriptorPath);
            ProcessDialogs(dialogsRawResourcesPath, target);

            GameLogger.Log("Finished generating dialogs!");

            FileHelper.CreateDirectoryPathIfNotExists(descriptorPath);

            // Create descriptor file to refresh the cache.
            File.Create(descriptorPath);

            return true;
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
                    asset.Name = StringHelper.CapitalizeFirstLetter(script.Name);

                    AddAsset(asset);
                }

                _dialogConverter.ReloadDialogWith(script, asset);
                SaveAsset(asset);
            }

            if (!string.IsNullOrEmpty(errors))
            {
                GameLogger.Error("Found error while compiling latest dialogue changes!");
                GameLogger.Error(errors);
            }
        }

        /// <summary>
        /// Find all the assets names for an asset type <paramref name="t"/>.
        /// </summary>
        /// <param name="t">The type that inherist from <see cref="GameAsset"/>.</param>
        public ImmutableDictionary<string, Guid> FindAllNamesForAssetWithGuid(Type t)
        {
            var builder = ImmutableDictionary.CreateBuilder<string, Guid>(StringComparer.OrdinalIgnoreCase);

            foreach (Type tt in ReflectionHelper.GetAllImplementationsOf(t))
            {
                if (_database.TryGetValue(tt, out HashSet<Guid>? assetGuids))
                {
                    foreach (Guid g in assetGuids)
                    {
                        builder[Path.GetFileNameWithoutExtension(_allAssets[g].Name)] = g;
                    }
                }
            }

            return builder.ToImmutable();
        }
    }
}