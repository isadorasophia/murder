namespace Murder.Serializer.Templating;

public class Templates
{
    public static SourceWriter GenerateJsonSerializerOptions(string projectPrefix)
    {
        SourceWriter writer = new(filename: $"{projectPrefix}SourceGenerationContext.g.cs");

        writer.WriteLine($$"""
            using System.Text.Json.Serialization;

            namespace Murder.Serialization;

            """);

        writer.WriteLine($$"""
            /// <summary>
            /// Serialization context for all the types within {{projectPrefix}}. You may find here all the:
            ///  - Components
            ///  - State machines
            ///  - Interactions
            ///  - Game assets
            ///  
            /// And any private fields that these types have.
            /// </summary>
            """);

        writer.WriteLine($$"""
            [JsonSerializable(typeof(Murder.Components.PositionComponent))] // start with something to test!
            [JsonSerializable(typeof(Murder.Components.MoveToComponent))] // start with something to test!
            public partial class {{projectPrefix}}SourceGenerationContext : JsonSerializerContext, IMurderSerializer
            {
            }
            """);

        return writer;
    }
}
