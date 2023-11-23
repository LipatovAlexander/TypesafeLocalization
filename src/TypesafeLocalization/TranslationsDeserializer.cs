using System.Collections.Immutable;
using System.Text.Json;
using Microsoft.CodeAnalysis;

namespace TypesafeLocalization;

public static class TranslationsDeserializer
{
    public static IReadOnlyList<Dictionary<string, string>> GetTranslations(
        SourceProductionContext context,
        ImmutableArray<string> translationsAsJson)
    {
        var translations = new List<Dictionary<string, string>>();

        foreach (var translationAsJson in translationsAsJson)
        {
            try
            {
                var translation = JsonSerializer.Deserialize<Dictionary<string, string>>(translationAsJson);

                if (translation is not null)
                {
                    translations.Add(translation);
                    continue;
                }

                var error = Diagnostic.Create(DiagnosticsDescriptors.TranslationDeserializationError, Location.None);
                context.ReportDiagnostic(error);
            }
            catch (JsonException ex)
            {
                var error = Diagnostic.Create(DiagnosticsDescriptors.TranslationDeserializationError, Location.None, ex.ToString());
                context.ReportDiagnostic(error);
            }
        }

        return translations;
    }
}
