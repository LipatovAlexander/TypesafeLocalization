using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace TypesafeLocalization;

[Generator]
public sealed class LocaleEnumGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var localizationsPaths = context.AdditionalTextsProvider
            .Where(text => text.Path.EndsWith(".i18n.json"))
            .Select((text, _) => text.Path)
            .Collect();

        context.RegisterSourceOutput(localizationsPaths, GenerateLocale);
    }

    private static void GenerateLocale(SourceProductionContext context, ImmutableArray<string> localizationsPaths)
    {
        var locales = new List<string>();

        foreach (var path in localizationsPaths)
        {
            var fileName = Path.GetFileNameWithoutExtension(path);
            var splitted = fileName.Split('.');

            if (splitted.Length != 3)
            {
                var error = Diagnostic.Create(DiagnosticsDescriptors.InvalidTranslationFileName, Location.None, path);
                context.ReportDiagnostic(error);
                continue;
            }

            var locale = splitted[1];
            locales.Add(locale);
        }

        context.AddSource("Locale.g.cs", SourceGenerationHelper.LocaleEnum(locales));
    }
}
