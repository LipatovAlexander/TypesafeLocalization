using System.Collections.Immutable;

namespace TypesafeLocalization;

public sealed record LocalizationContext(Translation BaseTranslation, IReadOnlyList<Translation> Translations);

public sealed record LocalizationConfiguration(Locale BaseLocale)
{
    public static readonly LocalizationConfiguration Default = new(new Locale("en"));
};

public sealed record Locale(string OriginalName)
{
    public override string ToString()
    {
        return OriginalName
            .Replace("-", "")
            .Replace("_", "");
    }
}

public sealed record Translation(Locale Locale, ImmutableSortedDictionary<string, string> Dictionary);
