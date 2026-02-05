using ImGuiNET;
using Microsoft.VisualBasic.FileIO;
using Murder.Assets;
using Murder.Assets.Localization;
using Murder.Core.Input;
using Murder.Diagnostics;
using Murder.Editor.Assets;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Utilities.Serialization;
using Murder.Serialization;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using static Murder.Editor.ImGuiExtended.SearchBox;

namespace Murder.Editor.Services;

internal static class EditorLocalizationServices
{
    public static LocalizedString? SearchLocalizedString()
    {
        LocalizationAsset localization = Game.Data.GetDefaultLocalization();

        SearchBoxSettings<Guid> settings = new(initialText: "Create localized string")
        {
            DefaultInitialization = ("New localized string", Guid.Empty)
        };

        Lazy<Dictionary<string, Guid>> candidates = new(() =>
        {
            Dictionary<string, Guid> result = [];

            foreach (LocalizedStringData data in localization.Resources)
            {
                if (data.IsGenerated)
                {
                    // Don't trust generated strings!
                    continue;
                }

                if (data.String is null)
                {
                    GameLogger.Error($"How is {data.Guid} null? This should never be possible. Did you mess the json manually?");
                    continue;
                }

                string key;
                if (data.String.Length > 32)
                {
                    key = data.String[..20] + "...";
                }
                else
                {
                    key = data.String;
                }

                if (result.ContainsKey(key))
                {
                    key += " (alt)";
                }

                result[key] = data.Guid;
            }

            return result;
        });

        if (SearchBox.Search(id: "s_", settings, values: candidates.Value, SearchBoxFlags.None, out Guid result))
        {
            if (result == Guid.Empty)
            {
                return AddNewResource();
            }

            AddExistingResource(result);
            return new(result);
        }

        return null;
    }

    public static LocalizedString? AddNewResource()
    {
        LocalizedString result = new(Guid.NewGuid());

        LocalizationAsset asset = Game.Data.GetDefaultLocalization();
        asset.AddResource(result.Id);

        EditorServices.SaveAssetWhenSelectedAssetIsSaved(asset.Guid);
        asset.FileChanged = true;

        return result;
    }

    private static void AddExistingResource(Guid g)
    {
        LocalizationAsset asset = Game.Data.GetDefaultLocalization();
        asset.AddResource(g);

        EditorServices.SaveAssetWhenSelectedAssetIsSaved(asset.Guid);
        asset.FileChanged = true;
    }

    /// <summary>
    /// Draw pop up with notes for this resource string.
    /// </summary>
    public static void DrawNotesPopup(Guid g)
    {
        LocalizationAsset asset = Game.Data.GetDefaultLocalization();
        if (asset.TryGetResource(g) is not LocalizedStringData data)
        {
            return;
        }

        ImGuiHelpers.HelpTooltip(string.IsNullOrEmpty(data.Notes) ? "(No notes)" : data.Notes);
        DrawNotesPopup(asset, g, data);
    }

    public static void DrawNotesPopup(LocalizationAsset asset, Guid g, LocalizedStringData data)
    {
        if (ImGui.BeginPopup($"notes_{g}"))
        {
            string text = data.Notes ?? string.Empty;
            if (ImGui.InputText("##notes_name", ref text, 1024, ImGuiInputTextFlags.AutoSelectAll))
            {
                asset.SetResource(data with { Notes = text });
            }

            if (ImGui.Button("Ok!") || Game.Input.Pressed(MurderInputButtons.Submit))
            {
                ImGui.CloseCurrentPopup();
            }

            ImGui.EndPopup();
        }
    }

    public static FilterLocalizationAsset GetFilterLocalizationAsset()
    {
        EditorSettingsAsset settings = Architect.EditorSettings;

        FilterLocalizationAsset? asset = Game.Data.TryGetAsset<FilterLocalizationAsset>(settings.LocalizationFilter);
        if (asset is null)
        {
            foreach ((_, GameAsset a) in Architect.EditorData.FilterAllAssets(typeof(FilterLocalizationAsset)))
            {
                asset = (FilterLocalizationAsset)a;
                break;
            }

            if (asset is null)
            {
                // Otherwise, this means we need to actually create one...
                asset = new();
                asset.Name = "_LocFilter";

                Architect.EditorData.SaveAsset(asset);
            }

            settings.LocalizationFilter = asset.Guid;
        }

        return asset;
    }

    public static ImmutableArray<(string, AssetInfoPropertiesForEditor)> GetLocalizationCandidates(string name)
    {
        FilterLocalizationAsset? asset = GetFilterLocalizationAsset();
        return asset.GetLocalizationCandidates(name);
    }

    public static bool ValidateStrings(LocalizationAsset asset)
    {
        string fullLocalizationPath = LocalizationExporter.GetFullRawLocalizationPath("ref");
        if (!File.Exists(fullLocalizationPath))
        {
            GameLogger.Error($"Missing file to validate from at: {fullLocalizationPath}");
            return false;
        }

        try
        {
            using TextFieldParser parser = new(fullLocalizationPath);

            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");

            int row = 0;
            while (!parser.EndOfData)
            {
                row++;

                // Read tokens for this row
                string[]? tokens = parser.ReadFields();
                if (tokens is null || tokens[0].StartsWith("Guid") || tokens[0].StartsWith('#'))
                {
                    continue;
                }

                if (string.IsNullOrEmpty(tokens[0]))
                {
                    continue;
                }

                if (tokens.Length < 3)
                {
                    GameLogger.Warning($"Skipping row {row} from {fullLocalizationPath}.");
                }

                Guid guid;
                try
                {
                    guid = Guid.Parse(tokens[0]);
                }
                catch
                {
                    GameLogger.Warning($"Skipping row {row} from {fullLocalizationPath}.");
                    continue;
                }

                /* not really used? */
                string text = tokens[2];

                LocalizedStringData? originalData = asset.TryGetResource(guid);
                if (originalData is null)
                {
                    GameLogger.Warning($"Unable to found: {guid}, {text}");
                }
                else if (!string.Equals(originalData?.String, text))
                {
                    GameLogger.Warning($"Mismatch on {guid}!");
                    GameLogger.Warning($"Current (in-game): {originalData?.String}");
                    GameLogger.Warning($"Resource file: {text}");
                }
            }
        }
        catch (IOException)
        {
            GameLogger.Warning($"Unable to open the file: {fullLocalizationPath}.");
        }
        catch (MalformedLineException)
        {

        }

        return true;
    }

    public static void ReorderResources(LocalizationAsset localization)
    {
        GameLogger.Log("Started reordering resources...");

        // this is only supported from the mode with all assets.
        ImmutableArray<(string, AssetInfoPropertiesForEditor)> resources = 
            GetLocalizationCandidates(FilterLocalizationAsset.DefaultFilterName);

        int resourceIndex = 0;
        Dictionary<Guid, int> resourceToIndex = [];

        StringBuilder resource = new();
        foreach ((string _, AssetInfoPropertiesForEditor info) in resources)
        {
            if (!info.IsSelected)
            {
                continue;
            }

            // assets are ordered alphabetically
            foreach (AssetPropertiesForEditor assetInfo in info.Assets)
            {
                if (!assetInfo.Show)
                {
                    continue;
                }

                GameAsset? asset = Game.Data.TryGetAsset(assetInfo.Guid);
                if (asset is null)
                {
                    continue;
                }

                string json = FileManager.SerializeToJson(asset);
                foreach (LocalizedStringData data in localization.Resources)
                {
                    if (resourceToIndex.ContainsKey(data.Guid))
                    {
                        // already accounted
                        continue;
                    }

                    if (json.Contains(data.Guid.ToString()))
                    {
                        resourceToIndex[data.Guid] = resourceIndex;
                        resourceIndex++;
                    }
                }
            }
        }

        var reordered = localization.Resources.OrderBy(l =>
        {
            if (resourceToIndex.TryGetValue(l.Guid, out int index))
            {
                return index;
            }

            return 0;
        });

        localization.SetAllResources([.. reordered]);

        GameLogger.Log("Finished reordering resources!");
    }

    public static void PrunNonReferencedStrings(LocalizationAsset localization, string name, bool log)
    {
        GameLogger.Log("Starting pruning missing references...");

        ImmutableArray<(string, AssetInfoPropertiesForEditor)> resources = GetLocalizationCandidates(name);

        StringBuilder resource = new();
        foreach ((string _, AssetInfoPropertiesForEditor info) in resources)
        {
            if (!info.IsSelected)
            {
                continue;
            }

            foreach (AssetPropertiesForEditor assetInfo in info.Assets)
            {
                if (!assetInfo.Show)
                {
                    continue;
                }

                GameAsset? asset = Game.Data.TryGetAsset(assetInfo.Guid);
                if (asset is null)
                {
                    continue;
                }

                resource.Append(FileManager.SerializeToJson(asset));
            }
        }

        string content = resource.ToString();

        List<Guid> toDelete = [];
        foreach (LocalizedStringData data in localization.Resources)
        {
            if (!content.Contains(data.Guid.ToString()))
            {
                toDelete.Add(data.Guid);

                if (log)
                {
                    GameLogger.Log($"Pruning {data.String}...");
                }
            }
        }

        foreach (Guid g in toDelete)
        {
            localization.FileChanged = true;
            localization.RemoveResource(g, force: true);
        }

        if (log)
        {
            GameLogger.Log($"Completed pruning!");
        }
    }
}
