using Microsoft.CodeAnalysis;

namespace TypesafeLocalization;

public static class DiagnosticsDescriptors
{
    private const string Category = "TypesafeLocalization";

    public static readonly DiagnosticDescriptor MultipleConfigurationFilesFound = new(
        "TL0001",
        "Multiple configuration files found",
        "There should only be one configuration file",
        Category,
        DiagnosticSeverity.Warning,
        true);

    public static readonly DiagnosticDescriptor ConfigurationFileNotFound = new(
        "TL0002",
        "Configuration file not found",
        "Configuration file named TypesafeLocalizationConfig.json is required",
        Category,
        DiagnosticSeverity.Warning,
        true);

    public static readonly DiagnosticDescriptor InvalidConfigurationFile = new(
        "TL0003",
        "Invalid configuration file",
        "Configuration file is invalid or errors occurred when reading it",
        Category,
        DiagnosticSeverity.Warning,
        true);

    public static readonly DiagnosticDescriptor ConfigurationFileDeserializationError = new(
        "TL0004",
        "Configuration file deserialization error",
        "Configuration file could not be deserialized. Error: {0}.",
        Category,
        DiagnosticSeverity.Warning,
        true);

    public static readonly DiagnosticDescriptor InvalidTranslationFile = new(
        "TL0005",
        "Invalid translation file",
        "Translation file is invalid or errors occurred when reading it. Path: {0}.",
        Category,
        DiagnosticSeverity.Warning,
        true);

    public static readonly DiagnosticDescriptor TranslationFileDeserializationError = new(
        "TL0006",
        "Translation file deserialization error",
        "Translation file could not be deserialized. Path: {0}. Error: {1}.",
        Category,
        DiagnosticSeverity.Warning,
        true);

    public static readonly DiagnosticDescriptor BaseTranslationNotFound = new(
        "TL0006",
        "Base translation not found",
        "Base translation file not found. Expected file name: {0}.",
        Category,
        DiagnosticSeverity.Warning,
        true);

    public static readonly DiagnosticDescriptor FoundMultipleTranslationFilesForLocale = new(
        "TL0007",
        "Found multiple translation files for locale",
        "Found multiple translation files for locale {0}",
        Category,
        DiagnosticSeverity.Warning,
        true);
}
