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
            .Where(text => Path.GetFileName(text.Path) == "TypesafeLocalizationConfig.json")
            .Collect();

        var translations = context.AdditionalTextsProvider
            .Where(text => Regex.IsMatch(text.Path, @"^Translation\.[a-zA-Z\-_]*\.i18n\.json$"))
            .Collect();

        var valueProvider = configurations.Combine(translations);

        context.RegisterSourceOutput(valueProvider, GenerateCode);
    }

    private static void GenerateCode(
        SourceProductionContext context,
        (ImmutableArray<AdditionalText> Left, ImmutableArray<AdditionalText> Right) values)
    {
        var (configurationsTexts, translationsTexts) = values;

        var localizationInfoParsed = LocalizationInfoParser.TryParse(
            context,
            configurationsTexts,
            translationsTexts,
            out var localizationInfo);

        if (!localizationInfoParsed)
        {
            return;
        }

        context.AddSource("Locale.g.cs", SourceGenerationHelper.LocaleEnum(localizationInfo));
        context.AddSource("ILocalizer.g.cs", SourceGenerationHelper.LocalizerInterface(localizationInfo));
        context.AddSource("Localizer.g.cs", SourceGenerationHelper.LocalizerClass(localizationInfo));
    }
}
