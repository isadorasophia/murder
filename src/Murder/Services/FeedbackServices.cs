using Microsoft.Xna.Framework.Graphics;
using Murder.Assets;
using Murder.Diagnostics;
using Murder.Serialization;
using Murder.Utilities;
using System.IO.Compression;
using System.Net.NetworkInformation;
using System.Text;

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

    private static async Task<FileWrapper?> TryZipActiveSaveAsync()
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

    public static async Task<FileWrapper?> CreateTemporarySaveAndZipAsync()
    {
        SaveData? save = Game.Data.TryGetActiveSaveData();
        if (save is null)
        {
            return null;
        }

        string temporaryPath = Path.Combine(Path.GetTempPath(), $"nv_save_{Path.GetRandomFileName()}");
        _ = FileManager.GetOrCreateDirectory(temporaryPath);

        bool succeeded = await Game.Data.CreateTemporarySaveAsync(temporaryPath);
        if (!succeeded)
        {
            return null;
        }

        // send the same file formats as the actual save.
        temporaryPath = Path.Join(temporaryPath, $"{save.SaveSlot}");

        using Stream stream = new MemoryStream();
        try
        {
            ZipFile.CreateFromDirectory(temporaryPath, stream);
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

    public readonly struct ScreenshotFeedbackData
    {
        public readonly string Identifier;
        public readonly FileWrapper File;
        public readonly Texture Texture;

        public ScreenshotFeedbackData(string identifier, FileWrapper file, Texture texture)
        {
            Identifier = identifier;
            File = file;
            Texture = texture;
        }
    }

    public readonly struct FeedbackData
    {
        public readonly string Name { get; init; } = string.Empty;
        public readonly string Description { get; init; } = string.Empty;
        public readonly int MachineId { get; init; } = 0;

        public readonly ScreenshotFeedbackData?[]? Screenshots { get; init; } = null;

        public FeedbackData() { }
    }

    public static async Task<bool> SendSaveDataFeedbackAsync(FeedbackData data)
    {
        List<(string name, FileWrapper file)> files = [];

        FileWrapper? currentSaveData = await CreateTemporarySaveAndZipAsync();
        if (currentSaveData is not null)
        {
            files.Add(("c_save", currentSaveData.Value));
        }

        FileWrapper? zippedSaveData = await TryZipActiveSaveAsync();
        if (zippedSaveData is not null)
        {
            files.Add(("save", zippedSaveData.Value));
        }

        if (data.Screenshots is not null)
        {
            foreach (ScreenshotFeedbackData? screenshot in data.Screenshots)
            {
                if (screenshot is null)
                {
                    continue;
                }

                files.Add((screenshot.Value.Identifier, screenshot.Value.File));
            }
        }

        string computerName = GenerateFunnyName(data.MachineId);

        await SendFeedbackAsync(Game.Profile.FeedbackUrl, $"{StringHelper.CapitalizeFirstLetter(computerName)}: {data.Name}", data.Description, files);

        if (data.Screenshots is not null)
        {
            foreach (ScreenshotFeedbackData? screenshot in data.Screenshots)
            {
                screenshot?.Texture.Dispose();
            }
        }

        return true;
    }

    public static async Task SendFeedbackAsync(string url, string title, string description, IEnumerable<(string name, FileWrapper file)> files)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return; // Do not send feedback if the URL is not set.
        }

        using (HttpClient _client = new())
        using (MultipartFormDataContent content = new())
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

    public static ScreenshotFeedbackData? TryGetGameplayScreenshot(string name)
    {
        // Step 1: Capture the screenshot
        using Texture2D? screenshotTexture = RenderServices.CreateGameplayScreenshot();
        if (screenshotTexture is null)
        {
            return null;
        }

        if (TryGetScreenshotFromTexture(screenshotTexture, $"{name}_gameplay_screenshot") is not FileWrapper fw)
        {
            return null;
        }

        return new(
            identifier: "g_screenshot",
            file: fw,
            texture: screenshotTexture);
    }

    /// <summary>
    /// Should only be called on the main thread! This required the graphics device!
    /// </summary>
    public static ScreenshotFeedbackData? TryGetScreenshot(string name)
    {
        // Step 1: Capture the screenshot
        using Texture2D? screenshotTexture = RenderServices.CreateScreenshot();
        if (screenshotTexture is null)
        {
            return null;
        }

        if (TryGetScreenshotFromTexture(screenshotTexture, $"{name}_screenshot") is not FileWrapper fw)
        {
            return null;
        }

        return new(
            identifier: "screenshot",
            file: fw, 
            texture: screenshotTexture);
    }

    /// <summary>
    /// Should only be called on the main thread! This required the graphics device!
    /// </summary>
    private static FileWrapper? TryGetScreenshotFromTexture(Texture2D texture, string name)
    {
        try
        {
            // Convert the screenshot texture to byte array (PNG)
            byte[] imageBytes;
            using (var memoryStream = new MemoryStream())
            {
                texture.SaveAsPng(memoryStream, texture.Width, texture.Height);
                imageBytes = memoryStream.ToArray();
            }

            // Step 2: Return the FileWrapper containing the PNG file data
            return new FileWrapper(imageBytes, $"{name}.png");
        }
        catch (Exception ex)
        {
            // Handle exceptions
            GameLogger.Error($"An error occurred while getting the sccreenshot: {ex.Message}");
            return null;
        }
    }

    private static string GenerateFunnyName(int seed)
    {
        string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "ck", "k", "lh", "l", "m", "n", "p", "qu", "r", "s", "t", "v", "w", "x", "z", "bh", "cz", "th" };
        string[] vowels = { "a", "e", "i", "o", "u", "y", "aa", "oo" };

        Random random = new(seed);
        StringBuilder name = new();

        // Generate a name of a random length between 3 and 8
        int nameLength = random.Next(3, 9);

        for (int i = 0; i < nameLength; i++)
        {
            if (i % 2 == 0)
            {
                // Add a consonant
                name.Append(consonants[random.Next(consonants.Length)]);
            }
            else
            {
                // Add a vowel
                name.Append(vowels[random.Next(vowels.Length)]);
            }
        }

        return name.ToString();
    }
}