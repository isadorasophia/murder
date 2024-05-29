

namespace Murder.Attributes;

[Flags]
public enum EditorFieldFlags
{
    None,
    NoFilter = 1 << 0,
    SingleLine = 1 << 1,
}

public class EditorFieldPropertiesAttribute : Attribute
{
    public EditorFieldFlags Flags;

    public EditorFieldPropertiesAttribute(EditorFieldFlags flags)
    {
        Flags = flags;
    }
}
