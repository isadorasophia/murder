using Bang.Components;
using Murder.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Editor.Components;

public enum EditorTweenType
{
    Place,
    Lift,
    Move,
}
[DoNotPersistOnSave]
public readonly struct EditorTween : IComponent
{
    public readonly float StartTime;
    public readonly float Duration;
    public readonly EditorTweenType Type;

    public EditorTween(float start, float duration, EditorTweenType type)
    {
        StartTime = start;
        Duration = duration;
        Type = type;
    }
}
