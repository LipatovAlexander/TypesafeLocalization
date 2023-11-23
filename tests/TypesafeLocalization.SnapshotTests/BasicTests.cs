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

    [Fact]
    public async Task ShouldGenerateInterfaceCorrectly_WhenMultipleKeys()
    {
        const string translationPath = "Localization.i18n.json";
        const string translationJson = """
                                       {
                                           "Key1": "text",
                                           "Key2": "text",
                                           "Key3": "text"
                                       }
                                       """;

        var translationAdditionalText = new InMemoryAdditionalText(translationPath, translationJson);

        await TestHelper.Verify(translationAdditionalText);
    }

    [Fact]
    public async Task ShouldGenerateInterfaceCorrectly_WhenNoKeys()
    {
        const string translationPath = "Localization.i18n.json";
        const string translationJson = """
                                       {
                                       }
                                       """;

        var translationAdditionalText = new InMemoryAdditionalText(translationPath, translationJson);

        await TestHelper.Verify(translationAdditionalText);
    }
}
