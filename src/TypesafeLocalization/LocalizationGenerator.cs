﻿using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using TypesafeLocalization.Extensions;

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

        var localizationContext = PrepareLocalizationContext(context, configuration, baseTranslation, translations);

        context.AddSource("Locale.g.cs", SourceGenerationHelper.LocaleEnum(localizationContext));
        context.AddSource("ILocalizer.g.cs", SourceGenerationHelper.LocalizerInterface(localizationContext));
        context.AddSource("Localizer.g.cs", SourceGenerationHelper.LocalizerClass(localizationContext));
        context.AddSource("ILocalizerFactory.g.cs", SourceGenerationHelper.LocalizerFactoryInterface);
        context.AddSource("LocalizerFactory.g.cs", SourceGenerationHelper.LocalizerFactoryClass);
        context.AddSource("TranslationNotFoundException.g.cs", SourceGenerationHelper.TranslationNotFoundException);
    }

    private static LocalizationContext PrepareLocalizationContext(
        SourceProductionContext context,
        LocalizationConfiguration configuration,
        Translation baseTranslation,
        List<Translation> translations)
    {
        var duplicates = translations.FindDuplicates(t => t.Locale.OriginalName);

        foreach (var duplicate in duplicates)
        {
            var diagnostic = Diagnostics.DuplicateTranslationFile(duplicate);
            context.ReportDiagnostic(diagnostic);

            translations.Remove(duplicate);
        }

        foreach (var translation in translations)
        {
            var missingKeys = FindMissingKeys(baseTranslation, translation);
            var extraKeys = FindMissingKeys(translation, baseTranslation);

            foreach (var key in missingKeys)
            {
                var diagnostic = Diagnostics.LocalizationKeyMissing(key, translation.Locale);
                context.ReportDiagnostic(diagnostic);

                switch (configuration.Strategy)
                {
                    case Strategy.Skip:
                        baseTranslation.Dictionary.Remove(key);
                        break;
                    case Strategy.Fallback:
                        translation.Dictionary.Add(key, baseTranslation.Dictionary[key]);
                        break;
                    case Strategy.Throw:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            foreach (var key in extraKeys)
            {
                var diagnostic = Diagnostics.ExtraLocalizationKey(key, translation.Locale);
                context.ReportDiagnostic(diagnostic);

                translation.Dictionary.Remove(key);
            }
        }

        return new LocalizationContext(baseTranslation, translations);
    }

    private static IEnumerable<string> FindMissingKeys(Translation baseTranslation, Translation translation)
    {
        return baseTranslation.Dictionary.Keys.Except(translation.Dictionary.Keys);
    }
}
