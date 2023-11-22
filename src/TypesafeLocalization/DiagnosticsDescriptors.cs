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
}
