using Bang;

namespace Murder.Core;

public readonly struct Coroutine
{
    public readonly int Index;

    public Coroutine(int index)
    {
        Index = index;
    }

    public void Stop(World world)
    {
        (world as MonoWorld)?.StopCoroutine(this);
    }
}
