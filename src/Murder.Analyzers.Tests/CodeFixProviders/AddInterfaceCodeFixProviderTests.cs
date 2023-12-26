using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Murder.Analyzers.Analyzers;
using Murder.Analyzers.CodeFixProviders;
using Murder.Analyzers.Tests;

namespace Bang.Analyzers.Tests.CodeFixProviders;

using Verify = MurderCodeFixProviderVerifier<AttributeAnalyzer, AddInterfaceCodeFixProvider>;

[TestClass]
public sealed class AddInterfaceCodeFixProviderTests
{
    [TestMethod(displayName: "Non-Component types annotated with the RuntimeOnly attribute trigger a warning.")]
    public async Task AnnotatedNonComponents()
    {
        const string source = @"
using Bang;
using Bang.Components;
using Murder.Utilities.Attributes;

namespace BangAnalyzerTestNamespace;

[RuntimeOnly]
public readonly struct IncorrectRuntimeOnly { }";
        const string codeFix = @"
using Bang;
using Bang.Components;
using Murder.Utilities.Attributes;

namespace BangAnalyzerTestNamespace;

[RuntimeOnly]
public readonly struct IncorrectRuntimeOnly : IComponent { }";

        var expected = Verify.Diagnostic(AttributeAnalyzer.RuntimeOnlyAttributeOnNonComponent)
            .WithSeverity(DiagnosticSeverity.Error)
            .WithSpan(8, 2, 8, 13);

        await Verify.VerifyCodeFixAsync(source, expected, codeFix);
    }

    [TestMethod(displayName: "The code fix works even if there already is a base type.")]
    public async Task MultipleBaseTypes()
    {
        const string source = @"
using Bang;
using Bang.Components;
using Bang.Systems;
using Murder.Utilities.Attributes;

namespace BangAnalyzerTestNamespace;

[RuntimeOnly]
public readonly struct IncorrectRuntimeOnly : ISystem { }";
        const string codeFix = @"
using Bang;
using Bang.Components;
using Bang.Systems;
using Murder.Utilities.Attributes;

namespace BangAnalyzerTestNamespace;

[RuntimeOnly]
public readonly struct IncorrectRuntimeOnly : ISystem, IComponent { }";

        var expected = Verify.Diagnostic(AttributeAnalyzer.RuntimeOnlyAttributeOnNonComponent)
            .WithSeverity(DiagnosticSeverity.Error)
            .WithSpan(9, 2, 9, 13);

        await Verify.VerifyCodeFixAsync(source, expected, codeFix);
    }

    [TestMethod(displayName: "The code fix works even when there are multiple annotations.")]
    public async Task MultipleBaseTypesError()
    {
        const string source = @"
using System;
using Bang;
using Bang.Components;
using Murder.Utilities.Attributes;

namespace BangAnalyzerTestNamespace;

[Obsolete(""Blah""), RuntimeOnly]
public readonly struct IncorrectRuntimeOnly { }";
        const string codeFix = @"
using System;
using Bang;
using Bang.Components;
using Murder.Utilities.Attributes;

namespace BangAnalyzerTestNamespace;

[Obsolete(""Blah""), RuntimeOnly]
public readonly struct IncorrectRuntimeOnly : IComponent { }";

        var expected = Verify.Diagnostic(AttributeAnalyzer.RuntimeOnlyAttributeOnNonComponent)
            .WithSeverity(DiagnosticSeverity.Error)
            .WithSpan(9, 20, 9, 31);

        await Verify.VerifyCodeFixAsync(source, expected, codeFix);
    }
}