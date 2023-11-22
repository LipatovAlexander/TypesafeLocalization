namespace TypesafeLocalization.SnapshotTests;

[UsesVerify]
public class BasicTests
{
    [Fact]
    public async Task ShouldGenerateInterfaceCorrectly()
    {
        const string translationPath = "Localization.i18n.json";
        const string translationJson = """
                                       {
                                           "HelloWorld": "Hello, world!"
                                       }
                                       """;

        var translationAdditionalText = new InMemoryAdditionalText(translationPath, translationJson);

        await TestHelper.Verify(translationAdditionalText);
    }
}
