using Microsoft.Xna.Framework.Graphics;
using Murder.Diagnostics;
using Murder.Serialization;
using System.IO;

namespace Murder.Data;

public partial class GameDataManager
{
    public bool SupportScreenshots => _game?.SupportRecordings ?? false;

    public ScreenshotSupportedModes ScreenshotMode = ScreenshotSupportedModes.None;

    public virtual void RecordScreenshot(Texture2D screenshot)
    {
        if (!SupportScreenshots)
        {
            return;
        }

        switch (ScreenshotMode)
        {
            case ScreenshotSupportedModes.SaveAtPath:
                SaveScreenshot(screenshot);
                break;

            case ScreenshotSupportedModes.None:
                GameLogger.Log("Skipping recording screenshot.");
                break;

            default:
                GameLogger.Warning("Unsupported screenshot mode.");
                break;
        }
    }

    private void SaveScreenshot(Texture2D screenshot)
    {
        string fileName = $"screenshot-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.png";
        string filePath = Path.Combine(FileHelper.GetScreenshotFolder(), fileName);

        try
        {
            using (FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate))
            {
                // This is quite expensive, the best way to address this is
                // adding support to save as png asynchronously in FNA.
                screenshot.SaveAsPng(fileStream, screenshot.Width, screenshot.Height);
                fileStream.Close();
            }
        }
        catch (Exception ex)
        {
            GameLogger.Error($"An error occurred while getting the screenshot: {ex.Message}");
            return;
        }
    }
}

public enum ScreenshotSupportedModes
{
    None = 0,
    SaveAtPath = 1,
    CopyToClipboard = 2
}