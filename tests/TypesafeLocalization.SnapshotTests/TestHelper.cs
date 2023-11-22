using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace TypesafeLocalization.SnapshotTests;

public static class TestHelper
{
    public static Task Verify(params InMemoryAdditionalText[] additionalTexts)
    {
        var compilation = CSharpCompilation.Create("Tests");

        var generator = new LocalizerGenerator();

        var driver = CSharpGeneratorDriver.Create(generator)
            .AddAdditionalTexts(additionalTexts.Cast<AdditionalText>().ToImmutableArray())
            .RunGenerators(compilation);

        return Verifier.Verify(driver)
            .UseDirectory("Snapshots");
    }
}
