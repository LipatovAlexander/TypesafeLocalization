using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace TypesafeLocalization;

public static class SourceGenerationHelper
{
    public static SourceText Localizer(IEnumerable<string> keys)
    {
        var interfaceDeclaration = SyntaxFactory.InterfaceDeclaration("ILocalizer")
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .AddMembers(keys.Select(LocalizationMethod).ToArray());

        var fullString = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("TypesafeLocalization"))
            .AddMembers(interfaceDeclaration)
            .NormalizeWhitespace()
            .ToFullString();

        return SourceText.From(fullString, Encoding.UTF8);
    }

    private static MemberDeclarationSyntax LocalizationMethod(string key)
    {
        return SyntaxFactory.ParseMemberDeclaration($"string {key}();")!;
    }
}
