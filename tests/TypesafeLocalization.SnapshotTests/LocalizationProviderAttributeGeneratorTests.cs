namespace TypesafeLocalization.SnapshotTests;

[UsesVerify]
public sealed class LocalizationProviderAttributeGeneratorTests
{
    [Fact]
    public async Task ShouldGenerateAttributeCorrectly()
    {
        await TestHelper.Verify<LocalizationProviderAttributeGenerator>();
    }
}
