using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace TypesafeLocalization.SnapshotTests;

public sealed class InMemoryAdditionalText(string path, string text) : AdditionalText
{
    private readonly string _text = text;

    public override string Path { get; } = path;

    public override SourceText GetText(CancellationToken cancellationToken = new())
    {
        return SourceText.From(_text);
    }
}
