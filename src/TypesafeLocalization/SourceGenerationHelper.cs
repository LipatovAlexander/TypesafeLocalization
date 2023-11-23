using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace TypesafeLocalization;

public static class SourceGenerationHelper
{
    private const string Namespace = "TypesafeLocalization";

    public static SourceText LocaleEnum(IEnumerable<string> locales)
    {
        var syntaxes = locales
            .Select(NormalizeLocaleName)
            .Select(SyntaxFactory.EnumMemberDeclaration);

        var syntaxesList = new SeparatedSyntaxList<EnumMemberDeclarationSyntax>();
        syntaxesList = syntaxesList.AddRange(syntaxes);

        var enumDeclaration = SyntaxFactory.EnumDeclaration("Locale")
            .WithMembers(syntaxesList);

        var text = SyntaxFactory.FileScopedNamespaceDeclaration(SyntaxFactory.ParseName(Namespace))
            .AddMembers(enumDeclaration)
            .NormalizeWhitespace()
            .ToFullString();

        return SourceText.From(text, Encoding.UTF8);

        static string NormalizeLocaleName(string locale)
        {
            return locale
                .Replace("-", "")
                .Replace("_", "");
        }
    }

    public static SourceText LocalizationProviderAttribute()
    {
        const string text = $$"""
                              namespace {{Namespace}};

                              [System.AttributeUsage(System.AttributeTargets.Assembly)]
                              public sealed class LocalizationProviderAttribute : System.Attribute
                              {
                              }
                              """;

        return SourceText.From(text, Encoding.UTF8);
    }

    public static SourceText Localizer(IEnumerable<string> keys)
    {
        var interfaceDeclaration = SyntaxFactory.InterfaceDeclaration("ILocalizer")
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .AddMembers(keys.Select(LocalizationMethod).ToArray());

        var fullString = SyntaxFactory.FileScopedNamespaceDeclaration(SyntaxFactory.ParseName(Namespace))
            .AddMembers(interfaceDeclaration)
            .NormalizeWhitespace()
            .ToFullString();

        return SourceText.From(fullString, Encoding.UTF8);

        static MemberDeclarationSyntax LocalizationMethod(string key)
        {
            return SyntaxFactory.ParseMemberDeclaration($"string {key}();")!;
        }
    }
}
