namespace Murder.Diagnostics
{
    public class PerfTimeRecorder : IDisposable
    {
        private readonly DateTime _start;
        private readonly string _operationName;

        public PerfTimeRecorder(string name)
        {
            _start = DateTime.Now;
            _operationName = name;

            GameLogger.LogPerf($"Starting '{_operationName}'");
        }

        public void Dispose()
        {
            GameLogger.LogPerf($"Completed '{_operationName}' in {(DateTime.Now - _start).TotalSeconds:0.000} s.");
        }
    }
}