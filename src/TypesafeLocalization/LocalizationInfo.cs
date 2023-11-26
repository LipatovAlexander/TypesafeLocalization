using System.Collections.Immutable;

namespace TypesafeLocalization;

public sealed record LocalizationInfo(ImmutableSortedSet<string> Keys, IReadOnlyList<Translation> Translations);

public sealed record Translation(string Locale, ImmutableSortedDictionary<string, string> Dictionary);