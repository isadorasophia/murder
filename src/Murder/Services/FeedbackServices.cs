using Murder.Assets;
using Murder.Diagnostics;
using Murder.Serialization;
using Murder.Utilities;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace Murder.Services;

public static class FeedbackServices
{
    private static readonly HttpClient _client = new HttpClient();

    [Serializable]
    public readonly struct Feedback
    {
        public readonly DateTime Time;
        public readonly float Version;
        public readonly string Signature;

        public readonly string Message;
        public readonly string Log;

        [JsonConstructor]
        public Feedback(string message, string secretKey)
        {
            Time = DateTime.Now;
            Version = (Game.Instance as IMurderGame)?.Version ?? -1;
            Log = GameLogger.GetCurrentLog();

            Message = message;
            Signature = GenerateSignature(message, Time, secretKey);
        }

        private static string GenerateSignature(string message, DateTime time, string secretKey)
        {
            var encoding = new UTF8Encoding();
            string payload = $"{message}{time.ToUniversalTime():yyyyMMddHHmm}";
            using (var hmac = new HMACSHA256(encoding.GetBytes(secretKey)))
            {
                byte[] signatureBytes = hmac.ComputeHash(encoding.GetBytes(payload));
                return Convert.ToBase64String(signatureBytes);
            }
        }
    }

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

    public static async Task SendFeedbackAsync(string url, string message)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            // Do not send feedback if the URL is not set.
            return;
        }

        var feedback = new Feedback(message, Game.Profile.FeedbackKey);
        string json = FileHelper.GetSerializedJson(feedback);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            HttpResponseMessage response = await _client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            GameLogger.Log($"Feedback sent: {responseBody}");
        }
        catch (HttpRequestException e)
        {
            GameLogger.Error($"Feedback fail: {e.Message}");
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
        await stream.ReadAsync(buffer, offset: 0, count: buffer.Length);

        return new FileWrapper(buffer, $"{save.Name}_save.zip");
    }

    public static async Task<bool> SendSaveDataFeedbackAsync(string url, string message)
    {
        FileWrapper? zippedSaveData = await TryZipActiveSaveAsync();
        if (zippedSaveData is null)
        {
            return false;
        }

        await SendFeedbackAsync(url, message, zippedSaveData.Value);
        return true;
    }

    public static async Task SendFeedbackAsync(string url, string message, params FileWrapper[] files)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return; // Do not send feedback if the URL is not set.
        }

        Feedback feedback = new(message, Game.Profile.FeedbackKey);
        string json = FileHelper.GetSerializedJson(feedback);

        using (MultipartFormDataContent content = new())
        {
            content.Add(new StringContent(json, Encoding.UTF8, "application/json"), "feedback");

            foreach (FileWrapper f in files)
            {
                content.Add(new ByteArrayContent(f.Bytes, 0, f.Bytes.Length), "file", f.Name);
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
                GameLogger.Error($"Feedback fail: {e.Message}");
            }
        }
    }
}
