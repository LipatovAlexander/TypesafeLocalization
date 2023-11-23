﻿namespace TypesafeLocalization.SnapshotTests;

[UsesVerify]
public sealed class LocaleEnumGeneratorTests
{
    [Fact]
    public async Task ShouldNotGenerateEnum_WhenNoTranslationsFound()
    {
        await TestHelper.Verify<LocaleEnumGenerator>();
    }

    [Fact]
    public async Task ShouldGenerateEnum_WhenOneTranslationFound()
    {
        const string translationPath = "Localization.en.i18n.json";
        const string translationJson = """
                                       {
                                       }
                                       """;

        var translationAdditionalText = new InMemoryAdditionalText(translationPath, translationJson);

        await TestHelper.Verify<LocaleEnumGenerator>(translationAdditionalText);
    }

    [Fact]
    public async Task ShouldGenerateEnum_WhenMultipleTranslationFound()
    {
        const string translationPath1 = "Localization.en.i18n.json";
        const string translationPath2 = "Localization.ru.i18n.json";
        const string translationJson = """
                                       {
                                       }
                                       """;

        var translationAdditionalText1 = new InMemoryAdditionalText(translationPath1, translationJson);
        var translationAdditionalText2 = new InMemoryAdditionalText(translationPath2, translationJson);

        await TestHelper.Verify<LocaleEnumGenerator>(translationAdditionalText1, translationAdditionalText2);
    }

    [Fact]
    public async Task ShouldGenerateEnum_WhenKeyWithSeparatorFound()
    {
        const string translationPath1 = "Localization.en-US.i18n.json";
        const string translationPath2 = "Localization.ru_RU.i18n.json";
        const string translationJson = """
                                       {
                                       }
                                       """;

        var translationAdditionalText1 = new InMemoryAdditionalText(translationPath1, translationJson);
        var translationAdditionalText2 = new InMemoryAdditionalText(translationPath2, translationJson);

        await TestHelper.Verify<LocaleEnumGenerator>(translationAdditionalText1, translationAdditionalText2);
    }
}