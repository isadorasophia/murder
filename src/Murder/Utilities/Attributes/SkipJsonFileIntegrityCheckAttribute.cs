namespace Murder.Utilities;

/// <summary>
/// This means the packer will bypass checking the json and use the object equality instead.
/// Useful when there are dictionary fields in the object.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public class SkipJsonFileIntegrityCheckAttribute : Attribute
{
}
