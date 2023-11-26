using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace TypesafeLocalization;

[Generator]
public sealed class LocalizationGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var translations = context.AdditionalTextsProvider
            .Where(text => Regex.IsMatch(Path.GetFileName(text.Path), @"^Translation\.[a-zA-Z\-_]*\.i18n\.json$"))
            .Collect();

        context.RegisterSourceOutput(translations, GenerateCode);
    }

    private static void GenerateCode(SourceProductionContext context, ImmutableArray<AdditionalText> translationsTexts)
    {
        var localizationInfo = LocalizationInfoParser.Parse(context, translationsTexts);

        context.AddSource("Locale.g.cs", SourceGenerationHelper.LocaleEnum(localizationInfo));
        context.AddSource("ILocalizer.g.cs", SourceGenerationHelper.LocalizerInterface(localizationInfo));
        context.AddSource("Localizer.g.cs", SourceGenerationHelper.LocalizerClass(localizationInfo));
        context.AddSource("ILocalizerFactory.g.cs", SourceGenerationHelper.LocalizerFactoryInterface);
        context.AddSource("LocalizerFactory.g.cs", SourceGenerationHelper.LocalizerFactoryClass);
    }
}
