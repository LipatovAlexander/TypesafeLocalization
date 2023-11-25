using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace TypesafeLocalization.SnapshotTests;

public sealed class InMemoryAdditionalText : AdditionalText
{
    private readonly string _text;

    public InMemoryAdditionalText(string path, string text)
    {
        _text = text;
        Path = path;
    }

    public override string Path { get; }

    public override SourceText GetText(CancellationToken cancellationToken = new())
    {
        return SourceText.From(_text);
    }
}
