using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using TypesafeLocalization.Extensions;
using TypesafeLocalization.LightJson;
using TypesafeLocalization.LightJson.Serialization;

namespace TypesafeLocalization;

public static class LocalizationInfoParser
{
    public static LocalizationInfo Parse(
        SourceProductionContext context,
        IEnumerable<AdditionalText> translationsTexts)
    {
        var translations = ParseTranslations(context, translationsTexts);
        var keys = translations
            .Select(x => x.Dictionary.Keys)
            .IntersectAll()
            .ToImmutableSortedSet();

        return new LocalizationInfo(keys, translations);
    }

    private static IReadOnlyList<Translation> ParseTranslations(
        SourceProductionContext context,
        IEnumerable<AdditionalText> translationsTexts)
    {
        var result = new List<Translation>();

        foreach (var translationText in translationsTexts)
        {
            var text = translationText.GetText()?.ToString();

            if (text is null)
            {
                continue;
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
                var locale = filename.Split('.')[1]
                    .Replace("-", "")
                    .Replace("_", "");
                
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
            }
        }

        return result;
    }
}
