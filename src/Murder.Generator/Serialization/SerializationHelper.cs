using Newtonsoft.Json;

namespace Murder.Generator.Serialization
{
    internal class SerializationHelper
    {
        private readonly static JsonSerializerSettings _settings = new()
        {
            Formatting = Formatting.Indented,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        public static async ValueTask Serialize(
            Descriptor descriptor,
            string path)
        {
            string json = JsonConvert.SerializeObject(descriptor, _settings);
            await File.WriteAllTextAsync(path, json);
        }

        public static async ValueTask<Descriptor> DeserializeAsDescriptor(string path)
        {
            string json = await File.ReadAllTextAsync(path);

            Descriptor? descriptor = JsonConvert.DeserializeObject<Descriptor>(json, _settings);
            if (descriptor is null)
            {
                Console.WriteLine($"Unable to deserialize descriptor at path '{path}'.");
                throw new ArgumentException(path);
            }

            return descriptor;
        }
    }
}
