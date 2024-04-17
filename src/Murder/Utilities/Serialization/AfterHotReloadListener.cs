#if DEBUG
[assembly: System.Reflection.Metadata.MetadataUpdateHandler(typeof(Murder.Utilities.Serialization.AfterHotReloadApplied))]
namespace Murder.Utilities.Serialization;

internal static class AfterHotReloadApplied
{
    internal static void ClearCache(Type[]? _) { }

    internal static void UpdateApplication(Type[]? _)
    {
        // This is very interesting, but apparently if we keep the previous cache prior to hot reload the metadata
        // is no longer valid and we would be unable to populate the fields with reflection.
        SerializationHelper._types.Clear();
    }
}
#endif
