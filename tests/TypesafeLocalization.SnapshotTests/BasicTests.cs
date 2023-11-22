namespace TypesafeLocalization.SnapshotTests;

[UsesVerify]
public class BasicTests
{
    [Fact]
    public async Task ShouldGenerateInterfaceCorrectly()
    {
        const string translationPath = "Translations.json";
        const string translationJson = """
                                       {
                                           "HelloWorld": "Hello, world!"
                                       }
                                       """;

        var translationAdditionalText = new InMemoryAdditionalText(translationPath, translationJson);

        await TestHelper.Verify(translationAdditionalText);
    }
}
