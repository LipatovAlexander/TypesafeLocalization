using System.Collections.Immutable;

namespace TypesafeLocalization;

public sealed record LocalizationInfo(Configuration Configuration, Translation BaseTranslation, IReadOnlyList<Translation> Translations);

public sealed record Configuration(string BaseLocale);

public sealed record Translation(string Locale, ImmutableSortedDictionary<string, string> Dictionary);
