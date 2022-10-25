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
            string @namespace,
            List<ComponentDescriptor> components, 
            List<ComponentDescriptor> messages, 
            List<GenericComponentDescriptor> generics,
            string path)
        {
            Descriptor descriptor = new(
                @namespace,
                components.ToDictionary(c => c.Name, c => c), 
                messages.ToDictionary(m => m.Name, m => m), 
                generics.ToArray());

            string json = JsonConvert.SerializeObject(descriptor, _settings);

            await File.WriteAllTextAsync(path, json);
        }

        public static async ValueTask<Descriptor> DeserializeAsDescriptor(string path)
        {
            string json = await File.ReadAllTextAsync(path);

            return JsonConvert.DeserializeObject<Descriptor>(json, _settings);
        }

        public static async ValueTask<(ComponentDescriptor[] Components, ComponentDescriptor[] Messages, GenericComponentDescriptor[] Generics)> Deserialize(string path)
        {
            Descriptor descriptor = await DeserializeAsDescriptor(path);
            return (descriptor.Components.Values.ToArray(), descriptor.Messages.Values.ToArray(), descriptor.Generics.ToArray());
        }
    }
}
