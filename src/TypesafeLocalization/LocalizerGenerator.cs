using System.Collections.Immutable;
using System.Text.Json;
using Microsoft.CodeAnalysis;

namespace TypesafeLocalization;

[Generator]
public sealed class LocalizerGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var translations = context.AdditionalTextsProvider
            .Where(text => text.Path.EndsWith("translations.json",
                StringComparison.OrdinalIgnoreCase))
            .Select((text, token) => text.GetText(token)?.ToString())
            .Where(text => text is not null)!
            .Collect<string>();

        context.RegisterSourceOutput(translations, GenerateCode);
    }

    private static void GenerateCode(SourceProductionContext context, ImmutableArray<string> translationsAsJson)
    {
        var translations = GetTranslations(context, translationsAsJson);

        if (translations.Count == 0)
        {
            return;
        }

        var firstTranslation = translations.First();

        context.AddSource("ILocalizer.g.cs", SourceGenerationHelper.Localizer(firstTranslation.Keys));
    }

    private static IReadOnlyList<Dictionary<string, string>> GetTranslations(
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

                var error = Diagnostic.Create(DiagnosticsDescriptors.TranslationDeserializationError, null);
                context.ReportDiagnostic(error);
            }
            catch (JsonException ex)
            {
                var error = Diagnostic.Create(DiagnosticsDescriptors.TranslationDeserializationError, null, ex.ToString());
                context.ReportDiagnostic(error);
            }
        }

        return translations;
    }
}
