using Microsoft.CodeAnalysis;

namespace TypesafeLocalization;

public static class DiagnosticsDescriptors
{
    public static readonly DiagnosticDescriptor TranslationDeserializationError = new(
        "TL0001",
        "Translations could not be deserialized",
        "Translations could not be deserialized: {0}",
        "TypesafeLocalization",
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor InvalidTranslationFileName = new(
        "TL0002",
        "Invalid translation file name",
        "Translation file name should be formatted as {Namespace}.{Locale}.i18n.json but found {0}",
        "TypesafeLocalization",
        DiagnosticSeverity.Error,
        true);
}
