namespace Murder.Diagnostics;

/// <summary>
/// Track the latest update times.
/// </summary>
public class UpdateTimeTracker
{
    private const int _maxSize = 528;

    public readonly float[] Sample = new float[_maxSize];
    private int _index = 0;

    public int Length => _index + 1;

    public void Update(double dt)
    {
#if DEBUG
        if (_index + 1 >= _maxSize) 
        { 
            Array.Copy(Sample, 1, Sample, 0, _maxSize - 1);

            _index = _maxSize - 1;
        }
        else 
        {
            _index++; 
        }

        Sample[_index] = (float)dt * 1000;
#endif
    }
}
