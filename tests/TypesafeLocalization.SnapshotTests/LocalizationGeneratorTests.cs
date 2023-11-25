namespace TypesafeLocalization.SnapshotTests;

[UsesVerify]
public sealed class LocalizationGeneratorTests
{
    [Fact]
    public async Task CanGenerateCorrectCode()
    {
        const string configurationPath = "TypesafeLocalizationConfig.json";
        const string configurationJson = """
                                         {
                                             "BaseLocale": "en-US"
                                         }
                                         """;

        var configurationAdditionalText = new InMemoryAdditionalText(configurationPath, configurationJson);

        const string baseTranslationPath = "Translation.en-US.i18n.json";
        const string baseTranslationJson = """
                                           {
                                               "Key1": "Value 1",
                                               "Key2": "Value 2",
                                               "Key3": "Value 3"
                                           }
                                           """;

        var baseTranslationAdditionalText = new InMemoryAdditionalText(baseTranslationPath, baseTranslationJson);

        const string otherTranslationPath = "Translation.ru-RU.i18n.json";
        const string otherTranslationJson = """
                                            {
                                                "Key1": "Значение 1",
                                                "Key2": "Значение 2",
                                                "Key3": "Значение 3"
                                            }
                                            """;

        var otherTranslationAdditionalText = new InMemoryAdditionalText(otherTranslationPath, otherTranslationJson);

        await TestHelper.Verify<LocalizationGenerator>(
            configurationAdditionalText,
            baseTranslationAdditionalText,
            otherTranslationAdditionalText);
    }
}
