namespace TypesafeLocalization.SnapshotTests.Tests;

[UsesVerify]
public sealed class LocalizationProviderAttributeGeneratorTests
{
    [Fact]
    public async Task ConstantValue()
    {
        await TestHelper.Verify<LocalizationProviderAttributeGenerator>();
    }
}
