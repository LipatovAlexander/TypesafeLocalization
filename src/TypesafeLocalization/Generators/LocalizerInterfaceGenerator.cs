using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace TypesafeLocalization.Generators;

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

        var firstTranslation = translations.FirstOrDefault();
        var keys = firstTranslation?.Keys.ToArray() ?? Array.Empty<string>();

        context.AddSource("ILocalizer.g.cs", SourceGenerationHelper.Localizer(keys));
    }
}
