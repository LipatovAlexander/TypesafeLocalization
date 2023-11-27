namespace TypesafeLocalization.SnapshotTests;

[UsesVerify]
public sealed class LocalizationGeneratorTests
{
    [Fact]
    public async Task CanGenerateCorrectCode()
    {
        const string configurationPath = "Localization/LocalizationConfig.json";
        const string configurationJson = """
                                         {
                                             "baseLocale": "en-US"
                                         }
                                         """;

        var configurationAdditionalText = new InMemoryAdditionalText(configurationPath, configurationJson);

        const string baseTranslationPath = "Localization/Translation.en-US.json";
        const string baseTranslationJson = """
                                           {
                                               "Key1": "Value 1",
                                               "Key2": "Value 2",
                                               "Key3": "Value 3"
                                           }
                                           """;

        var baseTranslationAdditionalText = new InMemoryAdditionalText(baseTranslationPath, baseTranslationJson);

        const string secondTranslationPath = "Localization/Translation.ru-RU.json";
        const string secondTranslationJson = """
                                             {
                                                 "Key1": "Значение 1",
                                                 "Key2": "Значение 2",
                                                 "Key3": "Значение 3"
                                             }
                                             """;

        var secondTranslationAdditionalText = new InMemoryAdditionalText(secondTranslationPath, secondTranslationJson);

        await TestHelper.Verify<LocalizationGenerator>(
            configurationAdditionalText,
            baseTranslationAdditionalText,
            secondTranslationAdditionalText);
    }

    [Fact]
    public async Task CanGenerateCorrectCode_WhenBaseLocaleNotSpecified()
    {
        const string configurationPath = "Localization/LocalizationConfig.json";
        const string configurationJson = """
                                         {
                                             "baseLocale": "en"
                                         }
                                         """;

        var configurationAdditionalText = new InMemoryAdditionalText(configurationPath, configurationJson);

        const string baseTranslationPath = "Localization/Translation.en.json";
        const string baseTranslationJson = """
                                           {
                                               "Key1": "Value 1",
                                               "Key2": "Value 2",
                                               "Key3": "Value 3"
                                           }
                                           """;

        var baseTranslationAdditionalText = new InMemoryAdditionalText(baseTranslationPath, baseTranslationJson);

        await TestHelper.Verify<LocalizationGenerator>(configurationAdditionalText, baseTranslationAdditionalText);
    }

    [Fact]
    public async Task CanGenerateCorrectCode_WhenStrategyIsSkipAndKeyMissing()
    {
        const string configurationPath = "Localization/LocalizationConfig.json";
        const string configurationJson = """
                                         {
                                             "baseLocale": "en-US",
                                             "strategy": "skip"
                                         }
                                         """;

        var configurationAdditionalText = new InMemoryAdditionalText(configurationPath, configurationJson);

        const string baseTranslationPath = "Localization/Translation.en-US.json";
        const string baseTranslationJson = """
                                           {
                                               "Key1": "Value 1",
                                               "Key2": "Value 2",
                                               "Key3": "Value 3"
                                           }
                                           """;

        var baseTranslationAdditionalText = new InMemoryAdditionalText(baseTranslationPath, baseTranslationJson);

        const string secondTranslationPath = "Localization/Translation.ru-RU.json";
        const string secondTranslationJson = """
                                             {
                                                 "Key1": "Значение 1",
                                                 "Key2": "Значение 2"
                                             }
                                             """;

        var secondTranslationAdditionalText = new InMemoryAdditionalText(secondTranslationPath, secondTranslationJson);

        await TestHelper.Verify<LocalizationGenerator>(
            configurationAdditionalText,
            baseTranslationAdditionalText,
            secondTranslationAdditionalText);
    }

    [Fact]
    public async Task CanGenerateCorrectCode_WhenExtraKey()
    {
        const string configurationPath = "Localization/LocalizationConfig.json";
        const string configurationJson = """
                                         {
                                             "baseLocale": "en-US",
                                             "strategy": "skip"
                                         }
                                         """;

        var configurationAdditionalText = new InMemoryAdditionalText(configurationPath, configurationJson);

        const string baseTranslationPath = "Localization/Translation.en-US.json";
        const string baseTranslationJson = """
                                           {
                                               "Key1": "Value 1",
                                               "Key2": "Value 2",
                                               "Key3": "Value 3"
                                           }
                                           """;

        var baseTranslationAdditionalText = new InMemoryAdditionalText(baseTranslationPath, baseTranslationJson);

        const string secondTranslationPath = "Localization/Translation.ru-RU.json";
        const string secondTranslationJson = """
                                             {
                                                 "Key1": "Значение 1",
                                                 "Key2": "Значение 2",
                                                 "Key3": "Значение 3",
                                                 "Key4": "Значение 4"
                                             }
                                             """;

        var secondTranslationAdditionalText = new InMemoryAdditionalText(secondTranslationPath, secondTranslationJson);

        await TestHelper.Verify<LocalizationGenerator>(
            configurationAdditionalText,
            baseTranslationAdditionalText,
            secondTranslationAdditionalText);
    }
}
