using Murder.Diagnostics;
using Murder.Serialization;
using Newtonsoft.Json;

namespace Murder.Utilities
{
    public class SerializationHelper
    {
        /// <summary>
        /// This is really tricky. We currently use readonly structs everywhere, which is great
        /// and very optimized.
        /// HOWEVER, this is tricky in the editor serializer. We need an actual new copy
        /// so we don't modify any other IComponents which are using the same memory.
        /// In order to workaround that, we will literally serialize a new component and create its copy.
        /// </summary>
        public static T DeepCopy<T>(T c)
        {
            GameLogger.Verify(c is not null);

            var settings = FileHelper._settings;
            if (JsonConvert.DeserializeObject(JsonConvert.SerializeObject(c, settings), c.GetType(), settings) is not T obj)
            {
                throw new InvalidOperationException($"Unable to serialize {c.GetType().Name} for editor!?");
            }

            return obj;
        }
    }
}
