using Microsoft.CodeAnalysis;
using TypesafeLocalization.LightJson.Serialization;

namespace TypesafeLocalization;

file static class Descriptors
{
    public static readonly DiagnosticDescriptor ConfigurationFileDeserializationError = new(
        "TL0001",
        "Configuration file deserialization error",
        "Configuration file could not be deserialized. Path: {0}. Error: {0}.",
        "TypesafeLocalization",
        DiagnosticSeverity.Warning,
        true);

    public static readonly DiagnosticDescriptor BaseTranslationNotFound = new(
        "TL0002",
        "Base translation not found",
        "Translation file for base locale ({0}) not found",
        "TypesafeLocalization",
        DiagnosticSeverity.Warning,
        true);

    public static readonly DiagnosticDescriptor TranslationFileDeserializationError = new(
        "TL0003",
        "Translation file deserialization error",
        "Translation file could not be deserialized. Path: {0}. Error: {1}.",
        "TypesafeLocalization",
        DiagnosticSeverity.Warning,
        true);

    public static readonly DiagnosticDescriptor MultipleTranslationsFoundForLocale = new(
        "TL0004",
        "Multiple translations found for one locale",
        "Multiple translation files found for locale {0}",
        "TypesafeLocalization",
        DiagnosticSeverity.Warning,
        true);

    public static readonly DiagnosticDescriptor LocalizationKeyMissing = new(
        "TL0005",
        "Localization key missing",
        "Localization key {0} is missing from translation {1}",
        "TypesafeLocalization",
        DiagnosticSeverity.Warning,
        true);
}

public static class Diagnostics
{
    public static Diagnostic ConfigurationFileDeserializationError(string filePath, JsonParseException exception)
    {
        return Diagnostic.Create(Descriptors.ConfigurationFileDeserializationError, Location.None, filePath, exception.ToString());
    }

    public static Diagnostic BaseTranslationNotFound(Locale baseLocale)
    {
        return Diagnostic.Create(Descriptors.BaseTranslationNotFound, Location.None, baseLocale.OriginalName);
    }

    public static Diagnostic TranslationFileDeserializationError(string filePath, JsonParseException exception)
    {
        return Diagnostic.Create(Descriptors.TranslationFileDeserializationError, Location.None, filePath, exception.ToString());
    }

    public static Diagnostic MultipleTranslationsFoundForLocale(Locale locale)
    {
        return Diagnostic.Create(Descriptors.MultipleTranslationsFoundForLocale, Location.None, locale.OriginalName);
    }

    public static Diagnostic LocalizationKeyMissing(string key, Locale locale)
    {
        return Diagnostic.Create(Descriptors.LocalizationKeyMissing, Location.None, key, locale.OriginalName);
    }
}
