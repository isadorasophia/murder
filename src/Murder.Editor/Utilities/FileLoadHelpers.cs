﻿using Murder.Diagnostics;
using Murder.Serialization;

namespace Murder.Editor.Utilities
{
    internal static class FileLoadHelpers
    {
        /// <summary>
        /// Returns whether it should recalculate a <paramref name="sourcePath"/> based on the age of <paramref name="resultPath"/>.
        /// </summary>
        internal static bool ShouldRecalculate(string sourcePath, string resultPath)
        {
            if (!Directory.Exists(sourcePath))
            {
                return false;
            }

            if (!File.Exists(resultPath) && !Directory.Exists(resultPath))
            {
                // Atlas have not been created, repopulate!
                return true;
            }

            if (FileHelper.TryGetLastWrite(sourcePath) is DateTime lastSourceModified &&
                FileHelper.TryGetLastWrite(resultPath) is DateTime lastDestinationCreated)
            {
                return lastSourceModified > lastDestinationCreated;
            }

            GameLogger.Warning("Unable to get last write time of source root path!");
            return false;
        }

        internal static bool ShouldRecalculate(string sourcePath, DateTime lastTime)
        {
            if (!Directory.Exists(sourcePath))
            {
                return false;
            }

            if (FileHelper.TryGetLastWrite(sourcePath) is DateTime lastSourceModified)
            {
                return lastSourceModified > lastTime;
            }

            GameLogger.Warning("Unable to get last write time of source root path!");
            return false;
        }
    }
}