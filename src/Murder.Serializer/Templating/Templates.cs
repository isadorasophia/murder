using Microsoft.CodeAnalysis;
using Murder.Generator.Metadata;
using Murder.Serializer.Extensions;

namespace Murder.Serializer.Templating;

public class Templates
{
    public static SourceWriter GenerateContext(MetadataFetcher metadata, string projectPrefix)
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

        foreach (string t in metadata.SerializableTypes)
        {
            writer.WriteLine($$"""
                [JsonSerializable(typeof({{t}}))]
                """);
        }

        writer.WriteLine($$"""
            [JsonSerializable(typeof(Murder.Core.Geometry.Rectangle), TypeInfoPropertyName = "MurderRectangle")]
            [JsonSerializable(typeof(Murder.Core.Geometry.Point), TypeInfoPropertyName = "MurderPoint")]
            [JsonSerializable(typeof(Microsoft.Xna.Framework.Point), TypeInfoPropertyName = "XnaPoint")]
            [JsonSerializable(typeof(Murder.Core.Graphics.Color), TypeInfoPropertyName = "MurderColor")]
            public partial class {{projectPrefix}}SourceGenerationContext : JsonSerializerContext, IMurderSerializer
            {
            }
            """);

        return writer;
    }

    public static SourceWriter GenerateOptions(MetadataFetcher metadata, string projectPrefix)
    {
        SourceWriter writer = new(filename: $"{projectPrefix}SerializerOptionsExtensions.g.cs");

        writer.WriteLine($$"""
            using System.Text.Json.Serialization.Metadata;
            using System.Text.Json.Serialization;
            using System.Text.Json;
            using Murder.Utilities;

            namespace Murder.Serialization;

            /// <summary>
            /// Provides a json serializer that supports all the serializable types in {{projectPrefix}}.
            /// </summary>
            public static class {{projectPrefix}}SerializerOptionsExtensions
            {
                /// <summary>
                /// Default options that should be used when serializing or deserializing any components
                /// within the project.
                /// </summary>
                public static readonly JsonSerializerOptions Options = new()
                {
            """);

        if (metadata.ParentContext is INamedTypeSymbol parentOptions)
        {
            writer.WriteLine($$"""
                    TypeInfoResolver = JsonTypeInfoResolver.Combine({{parentOptions.FullyQualifiedName()}}.Default, {{projectPrefix}}SourceGenerationContext.Default)
            """);
        }
        else
        {
            writer.WriteLine($$"""
                    TypeInfoResolver = {{projectPrefix}}SourceGenerationContext.Default
            """);
        }

        writer.WriteLine($$"""
                        .WithModifiers(
                            static typeInfo =>
                            {
            """);

        bool first = true;
        foreach (var p in metadata.PolymorphicTypes)
        {
            if (first)
            {
                writer.WriteLine($$"""
                                    if (typeInfo.Type == typeof({{p.Key.FullyQualifiedName()}}))
                """);

                first = false;
            }
            else
            {
                writer.WriteLine($$"""
                                    else if (typeInfo.Type == typeof({{p.Key.FullyQualifiedName()}}))
                """);
            }

            writer.WriteLine($$"""
                                {
                                    typeInfo.PolymorphismOptions = new()
                                    {
                                        DerivedTypes =
                                        {
            """);

            foreach (var pp in p.Value)
            {
                writer.WriteLine($$"""
                                                new JsonDerivedType(typeof({{pp.QualifiedName}}), typeDiscriminator: "{{pp.QualifiedName}}"),
                """);
            }

            writer.WriteLine($$"""
                                        },
                                        UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor
                                    };
                                }
            """);
        }

        writer.WriteLine($$"""
                            },
                            SerializationHelper.ApplyModifierForPrivateFieldsAndGetters),
                    PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace,
                    IncludeFields = true,
                    WriteIndented = true,
                    IgnoreReadOnlyFields = false,
                    IgnoreReadOnlyProperties = true,
                    PropertyNameCaseInsensitive = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            """);

        writer.WriteLine($$"""
                Converters =
                {
        """);

        foreach (var info in metadata.ComplexDictionaries)
        {
            writer.WriteLine($$"""
                        new ComplexDictionaryConverter<{{info.Key.FullyQualifiedName()}}, {{info.Value.FullyQualifiedName()}}>(),
            """);
        }

        writer.WriteLine($$"""
                    new Murder.Serialization.JsonTypeConverter()
        """);

        writer.WriteLine($$"""
                }
        """);

        writer.WriteLine($$"""
                };
            }
            """);

        return writer;
    }
}
