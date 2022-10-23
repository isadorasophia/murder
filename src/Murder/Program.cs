using Murder.Diagnostics;
using System.Text;

namespace Murder
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                using Game game = new();
                game.Run();
            }
            catch (Exception ex) when (CaptureCrash(ex)) { }
        }

        public static bool CaptureCrash(Exception ex)
        {
            const string logFile = "crash.log";
            string currentDirectory = Environment.CurrentDirectory;
            string logFilePath = Path.Join(currentDirectory, logFile);

            StringBuilder content = new();
            foreach (string line in GameLogger.FetchLogs())
            {
                content.AppendLine(line);
            }

            content.AppendLine($"Exception thrown: '{ex.Message}'");
            content.AppendLine(ex.StackTrace);

            File.AppendAllTextAsync(logFilePath, content.ToString());

            return false;
        }
    }
}
