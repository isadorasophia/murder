using Murder.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Murder.Editor.Utilities.Serialization;

internal static class EditorSerializationHelper
{

    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code.", Justification = "Assembly is not trimmed.")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "We use source generators.")]
    public static T? TryDeepCopy<T>(T c)
    {
        GameLogger.Verify(c is not null);
        var serialized = JsonSerializer.Serialize(c, Game.Data.SerializationOptions);
        if (serialized == "{}")
        {
            // Serialization failed
            return default;
        }
        if (JsonSerializer.Deserialize<T>(serialized, Game.Data.SerializationOptions) is not T obj)
        {
            return default;
        }

        return obj;
    }
}
