namespace TypesafeLocalization.SnapshotTests;

[UsesVerify]
public sealed class LocalizationGeneratorTests
{
    [Fact]
    public async Task CanGenerateCorrectCode()
    {
        const string translationPath1 = "Translation.en-US.i18n.json";
        const string translationJson1 = """
                                        {
                                            "Key1": "Value 1",
                                            "Key2": "Value 2",
                                            "Key3": "Value 3"
                                        }
                                        """;

        var translationAdditionalText1 = new InMemoryAdditionalText(translationPath1, translationJson1);

        const string translationPath2 = "Translation.ru-RU.i18n.json";
        const string translationJson2 = """
                                        {
                                            "Key1": "Значение 1",
                                            "Key2": "Значение 2",
                                            "Key3": "Значение 3"
                                        }
                                        """;

        var translationAdditionalText2 = new InMemoryAdditionalText(translationPath2, translationJson2);

        await TestHelper.Verify<LocalizationGenerator>(translationAdditionalText1, translationAdditionalText2);
    }
}
