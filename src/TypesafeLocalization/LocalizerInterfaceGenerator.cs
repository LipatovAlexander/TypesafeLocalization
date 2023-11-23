using System.Collections.Immutable;
using System.Text.Json;
using Microsoft.CodeAnalysis;

namespace TypesafeLocalization;

[Generator]
public sealed class LocalizerInterfaceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var translations = context.AdditionalTextsProvider
            .Where(text => text.Path.EndsWith(".i18n.json"))
            .Select((text, token) => text.GetText(token)?.ToString())
            .Where(text => text is not null)!
            .Collect<string>();

        context.RegisterSourceOutput(translations, GenerateCode);
    }

    private static void GenerateCode(SourceProductionContext context, ImmutableArray<string> translationsAsJson)
    {
        var translations = TranslationsDeserializer.GetTranslations(context, translationsAsJson);

        if (translations.Count == 0)
        {
            return;
        }

        var firstTranslation = translations.First();

        context.AddSource("ILocalizer.g.cs", SourceGenerationHelper.Localizer(firstTranslation.Keys));
    }
}
