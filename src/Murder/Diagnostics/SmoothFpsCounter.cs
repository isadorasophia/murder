namespace Murder.Diagnostics
{
    /// <summary>
    /// This will smooth the average FPS of the game.
    /// </summary>
    public class SmoothFpsCounter
    {
        private readonly int _sampleSize = 10;
        private readonly double[] _previousDeltaTime;

        private int _currentFrameIndex = 0;
        private double _totalSampleDeltaTime = 0;

        /// <summary>
        /// Latest FPS value.
        /// </summary>
        public double Value => Math.Round(_sampleSize / _totalSampleDeltaTime);

        public SmoothFpsCounter(int size) =>
            (_sampleSize, _previousDeltaTime) = (size, new double[size]);

        public void Update(double dt)
        {
            _currentFrameIndex++;

            if (_currentFrameIndex == _sampleSize) { _currentFrameIndex = 0; }

            _totalSampleDeltaTime -= _previousDeltaTime[_currentFrameIndex];
            _totalSampleDeltaTime += dt;

            _previousDeltaTime[_currentFrameIndex] = dt;
        }
    }
}