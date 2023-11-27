using Microsoft.CodeAnalysis;

namespace TypesafeLocalization;

public static class TranslationsValidator
{
    public static bool CheckForDuplicates(SourceProductionContext context, IEnumerable<Translation> translations)
    {
        var duplicates = translations
            .Select(x => x.Locale)
            .GroupBy(l => l.OriginalName)
            .SelectMany(g => g.Skip(1))
            .Distinct()
            .ToArray();

        if (duplicates.Length <= 0)
        {
            return true;
        }

        foreach (var locale in duplicates)
        {
            var diagnostic = Diagnostics.MultipleTranslationsFoundForLocale(locale);
            context.ReportDiagnostic(diagnostic);
        }

        return false;
    }

    public static bool CheckForNonExistentKeys(
        SourceProductionContext context,
        Translation baseTranslation,
        IEnumerable<Translation> translations)
    {
        var isValid = true;

        foreach (var translation in translations)
        {
            foreach (var key in baseTranslation.Dictionary.Keys)
            {
                if (translation.Dictionary.ContainsKey(key))
                {
                    continue;
                }

                var diagnostic = Diagnostics.LocalizationKeyMissing(key, translation.Locale);
                context.ReportDiagnostic(diagnostic);
                isValid = false;
            }
        }

        return isValid;
    }
}
