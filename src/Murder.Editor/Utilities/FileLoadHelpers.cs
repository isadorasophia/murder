using Murder.Diagnostics;
using Murder.Serialization;

namespace Murder.Editor.Utilities
{
    internal static class FileLoadHelpers
    {
        /// <summary>
        /// Returns whether it should recalculate a <paramref name="path"/> based on the age of <paramref name="descriptorFile"/>.
        /// </summary>
        internal static bool ShouldRecalculate(string path, string descriptorFile)
        {
            if (!File.Exists(descriptorFile))
            {
                // Atlas have not been created, repopulate!
                return true;
            }

            if (FileHelper.TryGetLastWrite(path) is DateTime lastSourceModified)
            {
                DateTime lastDestinationCreated = File.GetLastWriteTime(descriptorFile);
                return lastSourceModified > lastDestinationCreated;
            }

            GameLogger.Warning("Unable to get last write time of source root path!");
            return false;
        }
    }
}
