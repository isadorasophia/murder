using Murder.Generator.Metadata;
using Murder.Serializer.Extensions;

namespace Murder.Serializer.Templating;

public class Templates
{
    public static SourceWriter GenerateJsonSerializerOptions(MetadataFetcher metadata, string projectPrefix)
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

        foreach (var t in metadata.SerializableTypes)
        {
            writer.WriteLine($$"""
                [JsonSerializable(typeof({{t.QualifiedName}}))]
                """);
        }

        writer.WriteLine($$"""
            [JsonSerializable(typeof(Murder.Core.Geometry.Rectangle), TypeInfoPropertyName = "MurderRectangle")]
            [JsonSerializable(typeof(Murder.Core.Geometry.Point), TypeInfoPropertyName = "MurderPoint")]
            [JsonSerializable(typeof(Murder.Core.Graphics.Color), TypeInfoPropertyName = "MurderColor")]
            public partial class {{projectPrefix}}SourceGenerationContext : JsonSerializerContext, IMurderSerializer
            {
            }
            """);

        return writer;
    }
}
