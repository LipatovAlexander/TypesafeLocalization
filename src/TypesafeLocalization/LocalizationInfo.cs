namespace TypesafeLocalization;

public sealed record LocalizationInfo(Configuration Configuration, Translation BaseTranslation, IReadOnlyList<Translation> Translations);

public sealed record Configuration(string BaseLocale);

public sealed record Translation(string Locale, IReadOnlyList<LocalizationString> Strings);

public sealed record LocalizationString(string Key, string Value);
