using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Murder.Editor;
using System.Collections.Immutable;

namespace Murder.Analyzers.Tests;

/// <summary>
/// Verifier that includes Murder binaries to the project.
/// </summary>
/// <typeparam name="TAnalyzer">Analyzer under test.</typeparam>
public class MurderAnalyzerVerifier<TAnalyzer> : AnalyzerVerifier<TAnalyzer, MurderAnalyzerTest<TAnalyzer>, MSTestVerifier>
    where TAnalyzer : DiagnosticAnalyzer, new()
{ }

/// <summary>
/// Implementation of CSharpAnalyzerTest that uses net7.0 (not enabled by default
/// as of now and needed for Murder) and includes the Murder dlls.
/// </summary>
/// <typeparam name="TAnalyzer">Analyzer under test.</typeparam>
public sealed class MurderAnalyzerTest<TAnalyzer> : CSharpAnalyzerTest<TAnalyzer, MSTestVerifier>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    public MurderAnalyzerTest()
    {
        var murderReference = MetadataReference.CreateFromFile(typeof(IMurderGame).Assembly.Location);
        var murderEditorReference = MetadataReference.CreateFromFile(typeof(IMurderArchitect).Assembly.Location);
        TestState.AdditionalReferences.Add(murderReference);
        TestState.AdditionalReferences.Add(murderEditorReference);
        ReferenceAssemblies = Net.Net70;
    }
}

/// <summary>
/// Verifier that includes Murder binaries to the project.
/// </summary>
/// <typeparam name="TCodeFix">CodeFixProvider under test.</typeparam>
/// <typeparam name="TAnalyzer">Analyzer under test.</typeparam>
public class MurderCodeFixProviderVerifier<TAnalyzer, TCodeFix> :
    CodeFixVerifier<TAnalyzer, TCodeFix, MurderCodeFixTest<TAnalyzer, TCodeFix>, MSTestVerifier>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFix : CodeFixProvider, new()
{ }

/// <summary>
/// Implementation of CSharpCodeFixTest that uses net7.0 (not enabled by default
/// as of now and needed for Murder) and includes the Murder dlls.
/// </summary>
/// <typeparam name="TAnalyzer">Analyzer under test.</typeparam>
/// <typeparam name="TCodeFix">CodeFixProvider under test.</typeparam>
public sealed class MurderCodeFixTest<TAnalyzer, TCodeFix> : CSharpCodeFixTest<TAnalyzer, TCodeFix, MSTestVerifier>
    where TCodeFix : CodeFixProvider, new()
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    public MurderCodeFixTest()
    {
        var murderReference = MetadataReference.CreateFromFile(typeof(IMurderGame).Assembly.Location);
        var murderEditorReference = MetadataReference.CreateFromFile(typeof(IMurderArchitect).Assembly.Location);
        TestState.AdditionalReferences.Add(murderReference);
        TestState.AdditionalReferences.Add(murderEditorReference);
        ReferenceAssemblies = Net.Net70;
    }
}

/// <summary>
/// This is kind of a hack because, as of now, net7.0 is not available out-of-the-box for analyzer testing.
/// Delete once this is not longer the case.
/// </summary>
internal static class Net
{
    private static readonly Lazy<ReferenceAssemblies> _lazyNet70 = new(() =>
        new ReferenceAssemblies(
            "net7.0",
            new PackageIdentity(
                "Microsoft.NETCore.App.Ref",
                "7.0.8"),
            Path.Combine("ref", "net7.0")
        )
    );
    public static ReferenceAssemblies Net70 => _lazyNet70.Value;

    private static readonly Lazy<ReferenceAssemblies> _lazyNet70Windows = new(() =>
        Net70.AddPackages(
            ImmutableArray.Create(
                new PackageIdentity("Microsoft.WindowsDesktop.App.Ref", "7.0.0-preview.5.22302.5"))));
    public static ReferenceAssemblies Net70Windows => _lazyNet70Windows.Value;
}