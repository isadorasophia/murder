using System.Text.Json.Serialization;

namespace Murder.Core.Dialogs;

/// <summary>
/// This is used to track variables created on the fly. This used to be an
/// object, but the serialization doesn't really like that, so I'll follow a 
/// similar pattern to other facts.
/// </summary>
public readonly struct OrphanBlackboardContext
{
    public readonly FactKind Kind = FactKind.Invalid;

    public readonly string? StrValue = null;

    public readonly int? IntValue = null;

    public readonly float? FloatValue = null;

    public readonly bool? BoolValue = null;

    public OrphanBlackboardContext() { }

    [JsonConstructor]
    public OrphanBlackboardContext(FactKind kind, bool? boolValue, int? intValue, float? floatValue, string? strValue)
    {
        (Kind, StrValue, IntValue, FloatValue, BoolValue) = (kind, strValue, intValue, floatValue, boolValue);
    }

    public OrphanBlackboardContext(object @value)
    {
        bool? @bool = null;
        int? @int = null;
        float? @float = null;
        string? @string = null;

        FactKind kind = FactKind.Invalid;

        // Do not propagate previous values.
        switch (@value)
        {
            case bool toBool:
                kind = FactKind.Bool;
                @bool = toBool;

                break;

            case int toInt:
                kind = FactKind.Int;
                @int = toInt;

                break;

            case float toFloat:
                kind = FactKind.Float;
                @float = toFloat;

                break;

            case string toString:
                kind = FactKind.String;
                @string = toString;

                break;
        }

        (Kind, StrValue, IntValue, FloatValue, BoolValue) = (kind, @string, @int, @float, @bool);
    }

    public object? GetValue()
    {
        return Kind switch
        {
            FactKind.Bool => BoolValue,
            FactKind.Int => IntValue,
            FactKind.Float => FloatValue,
            FactKind.String => StrValue,
            _ => null,
        };
    }
}
