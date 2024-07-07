using Murder.Assets;
using Murder.Diagnostics;
using Murder.Serialization;
using Murder.Utilities;
using System;
using System.Globalization;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace Murder.Services;

public static class FeedbackServices
{
    private static readonly HttpClient _client = new HttpClient();

    public readonly struct FileWrapper
    {
        public readonly byte[] Bytes;
        public readonly string Name;

        public FileWrapper(byte[] bytes, string name)
        {
            Bytes = bytes;
            Name = name;
        }
    }

    public static async Task<FileWrapper?> TryZipScreenshotAsync()
    {
        string saveName = Game.Data.TryGetActiveSaveData()?.Name ?? "Unknown";
        try
        {
            // Step 1: Capture the screenshot
            var screenshotTexture = RenderServices.CreateGameplayScreenShot();
            if (screenshotTexture is null)
            {
                return null;
            }

            // Convert the screenshot texture to byte array (PNG)
            byte[] imageBytes;
            using (var memoryStream = new MemoryStream())
            {
                screenshotTexture.SaveAsPng(memoryStream, screenshotTexture.Width, screenshotTexture.Height);
                imageBytes = memoryStream.ToArray();
            }

            // Step 2: Create a ZIP file containing the PNG image
            byte[] zipBytes;
            using (var zipMemoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Create, true))
                {
                    var screenshotEntry = archive.CreateEntry($"{saveName}_gameplay_screenshot.png");
                    using (var entryStream = screenshotEntry.Open())
                    {
                        await entryStream.WriteAsync(imageBytes, 0, imageBytes.Length);
                    }
                }

                zipBytes = zipMemoryStream.ToArray();
            }

            // Step 3: Return the FileWrapper containing the ZIP file data
            return new FileWrapper(zipBytes, $"{saveName}_screenshots.zip");
        }
        catch (Exception ex)
        {
            // Handle exceptions
            GameLogger.Error($"An error occurred: {ex.Message}");
            return null;
        }
    }

    public static async Task<FileWrapper?> TryGetGameplayScreenshotAsync()
    {
        string saveName = Game.Data.TryGetActiveSaveData()?.Name ?? "Unknown";
        try
        {
            // Step 1: Capture the screenshot
            using var screenshotTexture = RenderServices.CreateGameplayScreenShot();
            if (screenshotTexture is null)
            {
                return null;
            }

            // Convert the screenshot texture to byte array (PNG)
            byte[] imageBytes;
            using (var memoryStream = new MemoryStream())
            {
                screenshotTexture.SaveAsPng(memoryStream, screenshotTexture.Width, screenshotTexture.Height);
                imageBytes = memoryStream.ToArray();
            }

            // Step 2: Return the FileWrapper containing the PNG file data
            return new FileWrapper(imageBytes, $"{saveName}_gameplay_screenshot.png");
        }
        catch (Exception ex)
        {
            // Handle exceptions
            GameLogger.Error($"An error occurred: {ex.Message}");
            return null;
        }
    }
    public static async Task<FileWrapper?> TryGetScreenshotAsync()
    {
        string saveName = Game.Data.TryGetActiveSaveData()?.Name ?? "Unknown";
        try
        {
            // Step 1: Capture the screenshot
            using var screenshotTexture = RenderServices.CreateScreenShot();
            if (screenshotTexture is null)
            {
                return null;
            }

            // Convert the screenshot texture to byte array (PNG)
            byte[] imageBytes;
            using (var memoryStream = new MemoryStream())
            {
                screenshotTexture.SaveAsPng(memoryStream, screenshotTexture.Width, screenshotTexture.Height);
                imageBytes = memoryStream.ToArray();
            }

            // Step 2: Return the FileWrapper containing the PNG file data
            return new FileWrapper(imageBytes, $"{saveName}_screenshot.png");
        }
        catch (Exception ex)
        {
            // Handle exceptions
            GameLogger.Error($"An error occurred: {ex.Message}");
            return null;
        }
    }


    public static async Task<FileWrapper?> TryZipActiveSaveAsync()
    {
        SaveData? save = Game.Data.TryGetActiveSaveData();
        if (save is null || Path.GetDirectoryName(save.GetGameAssetPath()) is not string savepath ||
            !Directory.Exists(savepath))
        {
            return null;
        }

        using Stream stream = new MemoryStream();
        try
        {
            ZipFile.CreateFromDirectory(savepath, stream);
        }
        catch
        {
            return null;
        }

        byte[] buffer = new byte[stream.Length];
        stream.Position = 0;
        await stream.ReadAsync(buffer);

        return new FileWrapper(buffer, $"{save.Name}_save.zip");
    }

    public static async Task<bool> SendSaveDataFeedbackAsync(string title, string description)
    {
        var files = new List<(string name, FileWrapper file)>();
        FileWrapper? zippedSaveData = await TryZipActiveSaveAsync();
        if (zippedSaveData is not null)
        {
            files.Add(("save", zippedSaveData.Value));
        }

        FileWrapper? gameplayScreenshot = await TryGetGameplayScreenshotAsync();
        if (gameplayScreenshot is not null)
        {
            files.Add(("g_screenshot", gameplayScreenshot.Value));
        }

        FileWrapper? screenshot = await TryGetScreenshotAsync();
        if (screenshot is not null)
        {
            files.Add(("screenshot", screenshot.Value));
        }

        await SendFeedbackAsync(Game.Profile.FeedbackUrl, title, description, files.ToArray());
        return true;
    }

    public static async Task SendFeedbackAsync(string url, string title, string description, params (string name, FileWrapper file)[] files)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return; // Do not send feedback if the URL is not set.
        }


        using (HttpClient _client = new HttpClient())
        using (MultipartFormDataContent content = new MultipartFormDataContent())
        {
            content.Add(new StringContent("MurderEngineFeedback", Encoding.UTF8), "feedback");

            content.Add(new StringContent(title, Encoding.UTF8), "Title");
            content.Add(new StringContent(description, Encoding.UTF8), "Description");
            content.Add(new StringContent(GameLogger.GetCurrentLog(), Encoding.UTF8), "Log");

            foreach (var f in files)
            {
                var byteArrayContent = new ByteArrayContent(f.file.Bytes);
                byteArrayContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                content.Add(byteArrayContent, f.name, f.file.Name);
            }

            try
            {
                HttpResponseMessage response = await _client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                GameLogger.Log($"Feedback sent: {responseBody}");
            }
            catch (HttpRequestException e)
            {
                GameLogger.Warning($"Sending feedback failed: {e.Message}");
            }
        }
    }
}
