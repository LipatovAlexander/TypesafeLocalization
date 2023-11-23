namespace TypesafeLocalization.SnapshotTests.Tests;

[UsesVerify]
public class LocalizerInterfaceGeneratorTests
{
    [Fact]
    public async Task NoTranslations()
    {
        await TestHelper.Verify<LocalizerInterfaceGenerator>();
    }

    [Fact]
    public async Task OneKey()
    {
        const string translationPath = "Localization.i18n.json";
        const string translationJson = """
                                       {
                                           "HelloWorld": "Hello, world!"
                                       }
                                       """;

        var translationAdditionalText = new InMemoryAdditionalText(translationPath, translationJson);

        await TestHelper.Verify<LocalizerInterfaceGenerator>(translationAdditionalText);
    }

    [Fact]
    public async Task MultipleKeys()
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

        await TestHelper.Verify<LocalizerInterfaceGenerator>(translationAdditionalText);
    }

    [Fact]
    public async Task NoKeys()
    {
        const string translationPath = "Localization.i18n.json";
        const string translationJson = """
                                       {
                                       }
                                       """;

        var translationAdditionalText = new InMemoryAdditionalText(translationPath, translationJson);

        await TestHelper.Verify<LocalizerInterfaceGenerator>(translationAdditionalText);
    }
}
