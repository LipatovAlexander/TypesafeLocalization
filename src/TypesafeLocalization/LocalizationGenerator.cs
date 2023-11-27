using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace TypesafeLocalization;

[Generator]
public sealed class LocalizationGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var configurations = context.AdditionalTextsProvider
            .Where(text => text.Path.EndsWith("LocalizationConfig.json"))
            .Collect();
    
        var translations = context.AdditionalTextsProvider
            .Where(text => Regex.IsMatch(Path.GetFileName(text.Path), @"^Translation\.[a-zA-Z\-_]*\.json$"))
            .Collect();

        var valuesProvider = configurations.Combine(translations);

        context.RegisterSourceOutput(valuesProvider, GenerateCode);
    }

    private static void GenerateCode(
        SourceProductionContext context,
        (ImmutableArray<AdditionalText> Left, ImmutableArray<AdditionalText> Right) values)
    {
        var (configurationsTexts, translationsTexts) = values;

        var configuration = AdditionalFilesParser.ParseConfiguration(context, configurationsTexts);
        var translations = AdditionalFilesParser.ParseTranslations(context, translationsTexts);

        var baseTranslation = translations.FirstOrDefault(x => x.Locale == configuration.BaseLocale);

        if (baseTranslation is null)
        {
            var diagnostic = Diagnostics.BaseTranslationNotFound(configuration.BaseLocale);
            context.ReportDiagnostic(diagnostic);
            return;
        }

        if (!TranslationsValidator.CheckForDuplicates(context, translations))
        {
            return;
        }

        if (!TranslationsValidator.CheckForNonExistentKeys(context, baseTranslation, translations))
        {
            return;
        }

        var localizationContext = new LocalizationContext(baseTranslation, translations);

        context.AddSource("Locale.g.cs", SourceGenerationHelper.LocaleEnum(localizationContext));
        context.AddSource("ILocalizer.g.cs", SourceGenerationHelper.LocalizerInterface(localizationContext));
        context.AddSource("Localizer.g.cs", SourceGenerationHelper.LocalizerClass(localizationContext));
        context.AddSource("ILocalizerFactory.g.cs", SourceGenerationHelper.LocalizerFactoryInterface);
        context.AddSource("LocalizerFactory.g.cs", SourceGenerationHelper.LocalizerFactoryClass);
    }
}
