using Bang.Diagnostics;
using ImGuiNET;
using Murder.Assets;
using Murder.Assets.Localization;
using Murder.Core.Input;
using Murder.Data;
using Murder.Diagnostics;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Utilities;
using Murder.Serialization;
using Murder.Services;
using System.IO;
using System.IO.Compression;
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
                    continue;
                }

                result[key] = data.Guid;
            }

            return result;
        });

        if (SearchBox.Search(id: "s_", settings, values: candidates, SearchBoxFlags.None, out Guid result))
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

    public static void PrunNonReferencedStrings(LocalizationAsset localization)
    {
        GameLogger.Log("Starting pruning missing references...");

        StringBuilder resource = new();
        foreach (GameAsset asset in Game.Data.GetAllAssets())
        {
            string? path = asset.GetEditorAssetPath();
            if (path is null)
            {
                continue;
            }

            if (Game.Data.ShouldSkipAsset(path))
            {
                continue;
            }

            if (asset is LocalizationAsset)
            {
                continue;
            }

            resource.Append(FileManager.SerializeToJson(asset));
        }

        string content = resource.ToString();

        List<Guid> toDelete = [];
        foreach (LocalizedStringData data in localization.Resources)
        {
            if (!content.Contains(data.Guid.ToString()))
            {
                toDelete.Add(data.Guid);
                GameLogger.Log($"Pruning {data.String}");
            }
        }

        foreach (Guid g in toDelete)
        {
            localization.FileChanged = true;
            localization.RemoveResource(g, force: true);
        }
    }
}
