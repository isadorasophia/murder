using Murder.Core.Particles;
using Murder.Diagnostics;
using Murder.Serialization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Serialization;

namespace Murder.Services;

public class FeedbackServices
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

    public static async Task SendFeedbackAsync(string url, string message, string filePath)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return; // Do not send feedback if the URL is not set.
        }

        var feedback = new Feedback(message, Game.Profile.FeedbackKey);
        string json = FileHelper.GetSerializedJson(feedback);

        using (var content = new MultipartFormDataContent())
        {
            content.Add(new StringContent(json, Encoding.UTF8, "application/json"), "feedback");

            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                byte[] fileBytes = File.ReadAllBytes(filePath);
                content.Add(new ByteArrayContent(fileBytes, 0, fileBytes.Length), "file", Path.GetFileName(filePath));
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
    public static async Task SendFeedbackAsync(string url, string message, byte[] fileBytes, string filename)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return; // Do not send feedback if the URL is not set.
        }

        var feedback = new Feedback(message, Game.Profile.FeedbackKey);
        string json = FileHelper.GetSerializedJson(feedback);

        using (var content = new MultipartFormDataContent())
        {
            content.Add(new StringContent(json, Encoding.UTF8, "application/json"), "feedback");

            content.Add(new ByteArrayContent(fileBytes, 0, fileBytes.Length), "file", filename);

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
