using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using TypesafeLocalization.LightJson;
using TypesafeLocalization.LightJson.Serialization;

namespace TypesafeLocalization;

public static class LocalizationInfoParser
{
    public static bool TryParse(
        SourceProductionContext context,
        IReadOnlyList<AdditionalText> configurationsTexts,
        IReadOnlyList<AdditionalText> translationsTexts,
        out LocalizationInfo localizationInfo)
    {
        localizationInfo = default!;

        var configurationParsed = TryParseConfiguration(context, configurationsTexts, out var configuration);
        var translationsParsed = TryParseTranslations(context, translationsTexts, out var translations);

        if (!configurationParsed || !translationsParsed)
        {
            return false;
        }

        var baseTranslation = translations.FirstOrDefault(x => x.Locale == configuration.BaseLocale);

        if (baseTranslation is null)
        {
            var diagnostic = Diagnostic.Create(
                DiagnosticsDescriptors.BaseTranslationNotFound,
                Location.None,
                $"Translation.{configuration.BaseLocale}.i18n.json");
            context.ReportDiagnostic(diagnostic);
            return false;
        }

        var duplications = translations
            .Select(x => x.Locale)
            .GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToArray();

        if (duplications.Length > 0)
        {
            foreach (var duplication in duplications)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticsDescriptors.FoundMultipleTranslationFilesForLocale,
                    Location.None,
                    duplication);

                context.ReportDiagnostic(diagnostic);
            }

            return false;
        }

        localizationInfo = new LocalizationInfo(configuration, baseTranslation, translations);
        return true;
    }

    private static bool TryParseConfiguration(
        SourceProductionContext context,
        IReadOnlyList<AdditionalText> configurationsTexts,
        out Configuration configuration)
    {
        configuration = default!;

        if (configurationsTexts.Count > 1)
        {
            var diagnostic = Diagnostic.Create(DiagnosticsDescriptors.MultipleConfigurationFilesFound, Location.None);
            context.ReportDiagnostic(diagnostic);
            return false;
        }

        if (configurationsTexts.Count == 0)
        {
            var diagnostic = Diagnostic.Create(DiagnosticsDescriptors.ConfigurationFileNotFound, Location.None);
            context.ReportDiagnostic(diagnostic);
            return false;
        }

        var text = configurationsTexts[0].GetText()?.ToString();

        if (text is null)
        {
            var diagnostic = Diagnostic.Create(DiagnosticsDescriptors.InvalidConfigurationFile, Location.None);
            context.ReportDiagnostic(diagnostic);
            return false;
        }

        try
        {
            var json = JsonValue.Parse(text).AsJsonObject;
            var baseLocale = json["BaseLocale"].AsString;
            configuration = new Configuration(baseLocale);
            return true;
        }
        catch (JsonParseException exception)
        {
            var diagnostic = Diagnostic.Create(
                DiagnosticsDescriptors.ConfigurationFileDeserializationError,
                Location.None,
                exception.ToString());
            context.ReportDiagnostic(diagnostic);
            return false;
        }
    }

    private static bool TryParseTranslations(
        SourceProductionContext context,
        IReadOnlyList<AdditionalText> translationsTexts,
        out IReadOnlyList<Translation> translations)
    {
        var result = new List<Translation>();
        translations = result;

        foreach (var translationText in translationsTexts)
        {
            var text = translationText.GetText()?.ToString();

            if (text is null)
            {
                var diagnostic = Diagnostic.Create(DiagnosticsDescriptors.InvalidTranslationFile, Location.None, translationText.Path);
                context.ReportDiagnostic(diagnostic);
                return false;
            }

            try
            {
                var json = JsonValue.Parse(text).AsJsonObject;
                var translationDictionary = json
                    .AsDictionary()
                    .ToImmutableSortedDictionary(
                        x => x.Key,
                        x => x.Value.AsString);

                var filename = Path.GetFileNameWithoutExtension(translationText.Path);
                var locale = filename.Split('.')[1];

                var translation = new Translation(locale, translationDictionary);
                result.Add(translation);
            }
            catch (JsonParseException exception)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticsDescriptors.TranslationFileDeserializationError,
                    Location.None,
                    translationText.Path,
                    exception.ToString());
                context.ReportDiagnostic(diagnostic);
                return false;
            }
        }

        return true;
    }
}
