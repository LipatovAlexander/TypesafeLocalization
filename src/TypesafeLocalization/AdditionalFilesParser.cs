using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using TypesafeLocalization.LightJson;
using TypesafeLocalization.LightJson.Serialization;

namespace TypesafeLocalization;

public static class AdditionalFilesParser
{
    public static LocalizationConfiguration ParseConfiguration(SourceProductionContext context, IEnumerable<AdditionalText> additionalTexts)
    {
        var additionalText = additionalTexts.FirstOrDefault();
        var jsonString = additionalText?.GetText()?.ToString();

        if (additionalText is null || jsonString is null)
        {
            return LocalizationConfiguration.Default;
        }

        try
        {
            // TODO: Add validation
            var json = JsonValue.Parse(jsonString).AsJsonObject;
            var baseLocaleName = json["baseLocale"].AsString;

            return new LocalizationConfiguration(new Locale(baseLocaleName));
        }
        catch (JsonParseException exception)
        {
            var diagnostic = Diagnostics.ConfigurationFileDeserializationError(additionalText.Path, exception);
            context.ReportDiagnostic(diagnostic);
            return LocalizationConfiguration.Default;
        }
    }

    public static IReadOnlyList<Translation> ParseTranslations(
        SourceProductionContext context,
        IEnumerable<AdditionalText> translationsTexts)
    {
        var result = new List<Translation>();

        foreach (var translationText in translationsTexts)
        {
            var jsonString = translationText.GetText()?.ToString();

            if (jsonString is null)
            {
                continue;
            }

            try
            {
                var json = JsonValue.Parse(jsonString).AsJsonObject;
                var translationDictionary = json
                    .AsDictionary()
                    .ToImmutableSortedDictionary(
                        x => x.Key,
                        x => x.Value.AsString);

                var filename = Path.GetFileNameWithoutExtension(translationText.Path);
                var locale = new Locale(filename.Split('.')[1]);
                
                var translation = new Translation(locale, translationDictionary);
                result.Add(translation);
            }
            catch (JsonParseException exception)
            {
                var diagnostic = Diagnostics.TranslationFileDeserializationError(translationText.Path, exception);
                context.ReportDiagnostic(diagnostic);
            }
        }

        return result;
    }
}
