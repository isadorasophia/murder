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
        ReferenceAssemblies = Net.Net80;
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
        ReferenceAssemblies = Net.Net80;
    }
}

/// <summary>
/// This is kind of a hack because, as of now, net7.0 is not available out-of-the-box for analyzer testing.
/// Delete once this is not longer the case.
/// TODO: Update nuget packages once net8.0 is actually released.
/// </summary>
internal static class Net
{
    private static readonly Lazy<ReferenceAssemblies> _lazyNet80 = new(() =>
        new ReferenceAssemblies(
            "net8.0",
            new PackageIdentity(
                "Microsoft.NETCore.App.Ref",
                "8.0.0-rc.1.23419.4"),
            Path.Combine("ref", "net8.0")
        )
    );
    public static ReferenceAssemblies Net80 => _lazyNet80.Value;
}