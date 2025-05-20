using Murder.Assets.Localization;
using Microsoft.VisualBasic.FileIO;
using Murder.Diagnostics;
using System.Text;
using Murder.Serialization;
using Murder.Assets;
using Murder.Utilities;
using Murder.Editor.Services;

namespace Murder.Editor.Utilities.Serialization;

internal static class LocalizationExporter
{
    public static string GetFullRawLocalizationPath()
    {
#if NO_SOURCE

        return FileHelper.GetPath(Game.Profile.LocalizationPath);
#endif

        return Path.Combine(FileHelper.GetPath(
                Architect.EditorData.EditorSettings.RawResourcesPath),
                Game.Profile.LocalizationPath);
    }

    public static string GetFullRawLocalizationPath(string name) => Path.Combine(
        GetFullRawLocalizationPath(), $"{name}.csv");

    private static string Escape(string? text) => text is null ? string.Empty : text.Replace("\"", "\"\"");

    private static bool _isExporting = false;

    public static bool ExportToCsv(LocalizationAsset asset, string name)
    {
        _isExporting = true;

        LocalizationAsset reference = Architect.EditorData.GetDefaultLocalization();

        // create a copy so we don't modify the actual asset
        LocalizationAsset target = SerializationHelper.DeepCopy(asset);
        EditorLocalizationServices.PrunNonReferencedStrings(target, name, log: false);

        StringBuilder builder = new();

        builder.AppendLine("Guid,Speaker,Original,Translated,Notes");

        HashSet<Guid> dialogueResources = new();
        foreach (LocalizationAsset.ResourceDataForAsset data in target.DialogueResources)
        {
            foreach (LocalizedDialogueData g in data.DataResources)
            {
                dialogueResources.Add(g.Guid);
            }
        }

        foreach (LocalizedStringData data in target.Resources)
        {
            if (dialogueResources.Contains(data.Guid))
            {
                // Add this separately
                continue;
            }

            LocalizedStringData? referenceData = reference.TryGetResource(data.Guid);
            builder.AppendLine($"{data.Guid},\"No speaker\",\"{Escape(referenceData?.String)}\",\"{Escape(data.String)}\",\"{data.Notes}\"");
        }

        // Now, put all these right at the end of the document.
        // Why, do you ask? Absolutely no reason. It just seemed reasonable that generated strings came later.
        foreach (LocalizationAsset.ResourceDataForAsset dialogueData in target.DialogueResources)
        {
            // Sometimes, an entire dialogue might have been skipped out and its data has been removed from the resource.
            // we do a sanity check here.
            bool hasAnyLineAvailable = false;
            foreach (LocalizedDialogueData localizedDialogueData in dialogueData.DataResources)
            {
                LocalizedStringData? data = target.TryGetResource(localizedDialogueData.Guid);
                if (data is not null)
                {
                    hasAnyLineAvailable = true;
                    break;
                }
            }

            if (!hasAnyLineAvailable)
            {
                continue;
            }

            if (Game.Data.TryGetAsset<CharacterAsset>(dialogueData.DialogueResourceGuid) is CharacterAsset characterAsset)
            {
                if (characterAsset.LocalizationNotes is null)
                {
                    builder.AppendLine($"## {characterAsset.GetSimplifiedName()} ##");
                }
                else
                {
                    builder.AppendLine($"## {characterAsset.GetSimplifiedName()} ##,\"{characterAsset.LocalizationNotes}\"");
                }
            }

            foreach (LocalizedDialogueData localizedDialogueData in dialogueData.DataResources)
            {
                LocalizedStringData? data = target.TryGetResource(localizedDialogueData.Guid);
                if (data is null)
                {
                    continue;
                }

                LocalizedStringData? referenceData = reference.TryGetResource(data.Value.Guid);

                string speakerName = Game.Data.TryGetAsset<SpeakerAsset>(localizedDialogueData.Speaker)?.SpeakerName ?? "No speaker";

                builder.AppendLine(
                    $"{data.Value.Guid},\"{speakerName}\",\"{Escape(referenceData?.String)}\",\"{Escape(data.Value.String)}\",\"{data.Value.Notes}\"");
            }
        }

        string fullLocalizationPath = GetFullRawLocalizationPath(target.Name);
        FileManager.CreateDirectoryPathIfNotExists(fullLocalizationPath);

        _ = File.WriteAllTextAsync(fullLocalizationPath, builder.ToString(), Encoding.UTF8);

        _isExporting = false;
        return true;
    }

    public static bool ImportFromCsv(LocalizationAsset asset)
    {
        if (_isExporting)
        {
            // do not import while we are exporting...
            return false;
        }

        string fullLocalizationPath = GetFullRawLocalizationPath(asset.Name);
        if (!File.Exists(fullLocalizationPath))
        {
            return false;
        }

        // create a temporary copy so it doesn't yell at excel opening your file.
        string tmpPath = Path.Combine(Path.GetTempPath(), asset.Name);

        try
        {
            File.Copy(fullLocalizationPath, tmpPath, true);

            using TextFieldParser parser = new(tmpPath);

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

                string translated = tokens.Length > 3 ? tokens[3] : string.Empty;
                string? notes = tokens.Length > 4 ? tokens[4] : null;
                asset.UpdateOrSetResource(guid, translated, notes);
            }
        }
        catch (IOException)
        {
            GameLogger.Warning($"Unable to open the file: {fullLocalizationPath}.");
        }
        catch (MalformedLineException)
        {

        }

        File.Delete(tmpPath);
        return true;
    }
}
