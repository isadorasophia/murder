namespace Murder.Core
{
    [Flags]
    public enum GridVisionStatus: uint
    {
        // Unexplored
        None = 0x0,

        // Seen at some point
        Explored = 0x1,

        // Currently being seen and explored
        Visible = 0x10,
    }
}
