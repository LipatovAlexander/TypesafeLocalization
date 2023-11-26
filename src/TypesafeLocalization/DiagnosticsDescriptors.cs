using Microsoft.CodeAnalysis;

namespace TypesafeLocalization;

public static class DiagnosticsDescriptors
{
    private const string Category = "TypesafeLocalization";

    public static readonly DiagnosticDescriptor TranslationFileDeserializationError = new(
        "TL0001",
        "Translation file deserialization error",
        "Translation file could not be deserialized. Path: {0}. Error: {1}.",
        Category,
        DiagnosticSeverity.Warning,
        true);
}
