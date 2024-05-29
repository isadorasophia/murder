﻿using Murder.Assets.Localization;
using Microsoft.VisualBasic.FileIO;
using Murder.Diagnostics;
using System.Text;
using Murder.Serialization;

namespace Murder.Editor.Utilities.Serialization;

internal static class LocalizationExporter
{
    private static string GetFullRawLocalizationPath(string name) => Path.Combine(
        FileHelper.GetPath(Architect.EditorData.EditorSettings.RawResourcesPath),
        Game.Profile.LocalizationPath,
         $"{name}.csv");

    public static bool ExportToCsv(LocalizationAsset asset)
    {
        LocalizationAsset reference = Architect.EditorData.GetDefaultLocalization();

        StringBuilder builder = new();

        builder.AppendLine("Guid,Original,Translated,Notes");

        HashSet<Guid> dialogueResources = new();
        foreach (LocalizationAsset.ResourceDataForAsset data in asset.DialogueResources)
        {
            foreach (Guid g in data.Resources)
            {
                dialogueResources.Add(g);
            }
        }

        foreach (LocalizedStringData data in asset.Resources)
        {
            if (dialogueResources.Contains(data.Guid))
            {
                // Add this separately
                continue;
            }

            LocalizedStringData? referenceData = reference.TryGetResource(data.Guid);
            builder.AppendLine($"{data.Guid},\"{referenceData?.String}\",\"{data.String}\",\"{data.Notes}\"");
        }

        // Now, put all these right at the end of the document.
        // Why, do you ask? Absolutely no reason. It just seemed reasonable that generated strings came later.
        foreach (LocalizationAsset.ResourceDataForAsset dialogueData in asset.DialogueResources)
        {
            foreach (Guid g in dialogueData.Resources)
            {
                LocalizedStringData? data = asset.TryGetResource(g);
                if (data is null)
                {
                    continue;
                }

                LocalizedStringData? referenceData = reference.TryGetResource(g);

                builder.AppendLine(
                    $"{data.Value.Guid},\"{referenceData?.String}\",\"{data.Value.String}\",\"{data.Value.Notes}\"");
            }
        }

        string fullLocalizationPath = GetFullRawLocalizationPath(asset.Name);
        FileManager.CreateDirectoryPathIfNotExists(fullLocalizationPath);

        _ = File.WriteAllTextAsync(fullLocalizationPath, builder.ToString(), Encoding.UTF8);
        return true;
    }

    public static bool ImportFromCsv(LocalizationAsset asset)
    {
        string fullLocalizationPath = GetFullRawLocalizationPath(asset.Name);
        if (!File.Exists(fullLocalizationPath))
        {
            return false;
        }

        using TextFieldParser parser = new(fullLocalizationPath);

        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(",");

        int row = 0;
        while (!parser.EndOfData)
        {
            row++;

            // Read tokens for this row
            string[]? tokens = parser.ReadFields();
            if (tokens is null || tokens[0].StartsWith("Guid"))
            {
                continue;
            }

            if (tokens.Length < 2)
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
            /* string original = tokens[1]; */

            string translated = tokens.Length > 2 ? tokens[2] : string.Empty;
            string? notes = tokens.Length > 3 ? tokens[3] : null;

            asset.UpdateOrSetResource(guid, translated, notes);
        }

        return true;
    }
}
