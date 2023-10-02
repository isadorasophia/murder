using Murder.Diagnostics;
using Murder.Editor.Importers;
using Murder.Serialization;
using System.Reflection;

namespace Murder.Editor.Data
{
    public partial class EditorDataManager
    {
        public ValueTask ReloadSprites()
        {
            if (LoadContentProgress is not null && !LoadContentProgress.IsCompleted)
            {
                // absolutely do not hot reload while loading.
                return default;
            }

            FetchResourcesForImporters(reload: true, skipIfNoChangesFound: true);
            LoadResourceImporters(reload: true, skipIfNoChangesFound: true);

            return default;
        }

        /// <summary>
        /// Load all the resource importers with a synchronous implementation.
        /// This requires that <see cref="FetchResourcesForImporters"/> has been called first.
        /// </summary>
        /// <param name="force">
        /// Whether it should force reloading all assets.
        /// </param>
        /// <param name="skipIfNoChangesFound">
        /// Whether it should run if there were no changes. If this value is true and force is true,
        /// this will not reload changes.
        /// </param>
        private void LoadResourceImporters(bool reload, bool skipIfNoChangesFound)
        {
            foreach (ResourceImporter importer in AllImporters)
            {
                if (importer.SupportsAsyncLoading && !reload)
                {
                    // Skip any async importers here (reloading doesn't really support async yet).
                    continue;
                }

                if (skipIfNoChangesFound && !importer.HasChanges)
                {
                    continue;
                }

                _ = importer.LoadStagedContentAsync(reload);
            }
        }

        private void FlushResourceImporters()
        {
            foreach (ResourceImporter importer in AllImporters)
            {
                if (!importer.SupportsAsyncLoading)
                {
                    // Skip any async importers here.
                    continue;
                }

                importer.Flush();
            }
        }

        /// <summary>
        /// Load all the resource importers with an asynchronous implementation.
        /// This requires that <see cref="FetchResourcesForImporters"/> has been called first.
        /// </summary>
        private async ValueTask LoadResourceImportersAsync(bool reload, bool skipIfNoChangesFound)
        {
            foreach (ResourceImporter importer in AllImporters)
            {
                if (!importer.SupportsAsyncLoading)
                {
                    // Skip any sync importers here.
                    continue;
                }

                if (skipIfNoChangesFound && !importer.HasChanges)
                {
                    continue;
                }

                await importer.LoadStagedContentAsync(reload);
            }
        }

        /// <summary>
        /// Initialize all resources tracked by the importers, if they changed since last import.
        /// </summary>
        private void FetchResourcesForImporters(bool reload, bool skipIfNoChangesFound)
        {
            // Making sure we have an input directory
            if (!Directory.Exists(FileHelper.GetPath(EditorSettings.GameSourcePath)))
            {
                GameLogger.Warning($"Please specify a valid \"Game Source Path\" in \"Editor Settings\". Unable to find the resources to build the atlas from.");
                return;
            }

            List<(ResourceImporter importer, ImporterSettingsAttribute filter)> importersWithFilters = new();
            foreach (ResourceImporter importer in AllImporters)
            {
                if (skipIfNoChangesFound && !importer.ShouldRecalculate())
                {
                    importer.ClearStage();
                    continue;
                }

                if (importer.GetType().GetCustomAttribute<ImporterSettingsAttribute>() is ImporterSettingsAttribute attribute)
                {
                    importersWithFilters.Add((importer, attribute));

                    // Prepare the importers for the files
                    importer.ClearStage();
                }
                else
                {
                    GameLogger.Error($"Importer {importer.GetType().Name} is missing an ImporterSettingsAttribute");
                }
            }

            if (importersWithFilters.Count == 0)
            {
                return;
            }

            DateTime lastTimeFetched = reload ? EditorSettings.LastHotReloadImport : EditorSettings.LastImported;

            string rawResourcesPath = FileHelper.GetPath(EditorSettings.RawResourcesPath);
            foreach (string file in Directory.GetFiles(rawResourcesPath, "*.*", SearchOption.AllDirectories))
            {
                // Skip files that begin with "_".
                if (Path.GetFileName(file).StartsWith('_'))
                {
                    continue;
                }

                foreach ((ResourceImporter importer, ImporterSettingsAttribute filter) in importersWithFilters)
                {
                    // Check if this file can be imported by current imported

                    // Fist, check the extension.
                    string extension = Path.GetExtension(file);
                    if (!filter.FileExtensions.Contains(extension))
                    {
                        continue;
                    }

                    // Now check the folder filter
                    string folder = Path.GetRelativePath(rawResourcesPath, Path.GetDirectoryName(file)!);
                    switch (filter.FilterType)
                    {
                        case FilterType.All:
                            break;

                        case FilterType.OnlyTheseFolders:
                            if (FileHelper.IsPathInsideOf(folder, filter.FilterFolders))
                            {
                                break;
                            }
                            continue;

                        case FilterType.ExceptTheseFolders:
                            if (!FileHelper.IsPathInsideOf(folder, filter.FilterFolders))
                            {
                                break;
                            }
                            continue;

                        case FilterType.None:
                            continue;
                    }

                    bool forceClean =
                        !Directory.Exists(importer.GetSourcePackedPath()) ||
                        !Directory.Exists(importer.GetSourceResourcesPath());

                    // If everything is good so far, put it on stage and check for changes
                    importer.StageFile(file, forceClean || File.GetLastWriteTime(file) > lastTimeFetched);
                    break;
                }
            }

            EditorSettings.LastHotReloadImport = DateTime.Now;

            if (!reload)
            {
                EditorSettings.LastImported = DateTime.Now;
            }

            SaveAsset(Architect.EditorSettings);
        }
    }
}
