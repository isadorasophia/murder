using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Murder.Analyzers.Analyzers;

namespace Murder.Analyzers.Tests.Analyzers;

using Verify = MurderAnalyzerVerifier<AttributeAnalyzer>;

[TestClass]
public sealed class ResourceAnalyzerTests
{
    [TestMethod(displayName: "Properly annotated ResourceImporter subtypes do not trigger the analyzer.")]
    public async Task ProperlyAnnotatedResourceImporter()
    {
        const string source = @"
using Murder.Editor.Assets;
using Murder.Editor.Importers;
using System.Threading.Tasks;

namespace MurderAnalyzerTestNamespace;

[ImporterSettings(FilterType.OnlyTheseFolders, new string[] { """" }, new string[] { ""."" })]
class CorrectImporter : ResourceImporter
{
    public override string RelativeSourcePath { get; } = """";
    public override string RelativeOutputPath { get; } = """";
    public override string RelativeDataOutputPath { get; } = """";
    
    public CorrectImporter(EditorSettingsAsset editorSettings) : base(editorSettings) { }

    public override ValueTask LoadStagedContentAsync(bool reload, bool skipIfNoChangesFound)
    {
        throw new System.NotImplementedException();
    }
    public override string GetSourcePackedAtlasDescriptorPath()
    {
        throw new System.NotImplementedException();
    }
}";
        await Verify.VerifyAnalyzerAsync(source);
    }

    [TestMethod(displayName: "A non-annotated ResourceImporter triggers an error.")]
    public async Task NonAnnotatedResourceImporter()
    {
        const string source = @"
using Murder.Editor.Assets;
using Murder.Editor.Importers;
using System.Threading.Tasks;

namespace MurderAnalyzerTestNamespace;

class IncorrectImporter : ResourceImporter
{
    public override string RelativeSourcePath { get; } = """";
    public override string RelativeOutputPath { get; } = """";
    public override string RelativeDataOutputPath { get; } = """";
    
    public IncorrectImporter(EditorSettingsAsset editorSettings) : base(editorSettings) { }

    public override ValueTask LoadStagedContentAsync(bool reload, bool skipIfNoChangesFound)
    {
        throw new System.NotImplementedException();
    }
    public override string GetSourcePackedAtlasDescriptorPath()
    {
        throw new System.NotImplementedException();
    }
}";
        var expected = Verify.Diagnostic(AttributeAnalyzer.ImporterSettingsAttribute)
            .WithSeverity(DiagnosticSeverity.Error)
            .WithSpan(8, 7, 8, 24);

        await Verify.VerifyAnalyzerAsync(source, expected);
    }

    [TestMethod(displayName: "Abstract messager Systems do not need the Filter or Message annotation.")]
    public async Task AbstractResourceImporter()
    {
        const string source = @"
using Murder.Editor.Assets;
using Murder.Editor.Importers;
using System.Threading.Tasks;

namespace MurderAnalyzerTestNamespace;

abstract class IncorrectImporter : ResourceImporter
{
    protected IncorrectImporter(EditorSettingsAsset editorSettings) : base(editorSettings) { }
}";
        await Verify.VerifyAnalyzerAsync(source);
    }

    [TestMethod(displayName: "Non-Component types annotated with the RuntimeOnly attribute trigger a warning.")]
    public async Task RuntimeOnlyAnnotatedNonComponents()
    {
        const string source = @"
using Bang;
using Bang.Components;
using Murder.Utilities.Attributes;

namespace BangAnalyzerTestNamespace;

[RuntimeOnly]
public readonly struct IncorrectRuntimeOnly { }";

        var expected = Verify.Diagnostic(AttributeAnalyzer.RuntimeOnlyAttributeOnNonComponent)
            .WithSeverity(DiagnosticSeverity.Error)
            .WithSpan(8, 2, 8, 13);

        await Verify.VerifyAnalyzerAsync(source, expected);
    }
}