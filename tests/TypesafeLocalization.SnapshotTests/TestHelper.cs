using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace TypesafeLocalization.SnapshotTests;

public static class TestHelper
{
    public static Task Verify<TGenerator>(params InMemoryAdditionalText[] additionalTexts)
        where TGenerator : IIncrementalGenerator, new()
    {
        var compilation = CSharpCompilation.Create("Tests");

        var generator = new TGenerator();

        var driver = CSharpGeneratorDriver.Create(generator)
            .AddAdditionalTexts(additionalTexts.Cast<AdditionalText>().ToImmutableArray())
            .RunGenerators(compilation);

        return Verifier.Verify(driver)
            .UseDirectory("Snapshots");
    }
}
