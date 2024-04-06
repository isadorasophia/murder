using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace Murder.Serializer.Templating;

public sealed class SourceWriter
{
    public readonly string Filename;

    private readonly StringBuilder _sb = new();

    public SourceWriter(string filename)
    {
        Filename = filename;
    }

    public void WriteLine(string text)
    {
        _sb.Append(text);
        _sb.AppendLine();
    }

    public SourceText ToSourceText() =>
        SourceText.From(_sb.ToString(), Encoding.UTF8);
}
