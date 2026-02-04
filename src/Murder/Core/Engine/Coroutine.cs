using Bang;

namespace Murder.Core;

public readonly struct Coroutine
{
    public readonly int Index = -1;

    public Coroutine() { }

    public Coroutine(int index)
    {
        Index = index;
    }

    public void Stop(World world)
    {
        if (Index == -1)
        {
            return;
        }

        (world as MonoWorld)?.StopCoroutine(this);
    }
}
