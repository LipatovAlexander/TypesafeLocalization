﻿using System.Text;

namespace TypesafeLocalization;

public static class SourceGenerationHelper
{
    private const string FileHeaderComment = """
                                             //------------------------------------------------------------------------------
                                             // <auto-generated>
                                             //     This code was generated by the TypesafeLocalization source generator
                                             //
                                             //     Changes to this file may cause incorrect behavior and will be lost if
                                             //     the code is regenerated.
                                             // </auto-generated>
                                             //------------------------------------------------------------------------------
                                             """;

    private const string Namespace = "namespace TypesafeLocalization;";

    public static string LocalizerInterface(LocalizationContext localizationContext)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine(FileHeaderComment);
        stringBuilder.AppendLine(Namespace);
        stringBuilder.AppendLine();

        stringBuilder.AppendLine("public interface ILocalizer");
        stringBuilder.AppendLine("{");

        foreach (var baseTranslation in localizationContext.BaseTranslation.Dictionary)
        {
            stringBuilder.AppendLine($"    string {baseTranslation.Key}();");
        }

        stringBuilder.AppendLine("}");

        return stringBuilder.ToString();
    }

    public static string LocaleEnum(LocalizationContext localizationContext)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine(FileHeaderComment);
        stringBuilder.AppendLine(Namespace);
        stringBuilder.AppendLine();

        stringBuilder.AppendLine("public enum Locale");
        stringBuilder.AppendLine("{");

        foreach (var translation in localizationContext.Translations)
        {
            stringBuilder.AppendLine($"    {translation.Locale},");
        }

        stringBuilder.AppendLine("}");

        return stringBuilder.ToString();
    }

    public static string LocalizerClass(LocalizationContext localizationContext)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine(FileHeaderComment);
        stringBuilder.AppendLine(Namespace);
        stringBuilder.AppendLine();

        stringBuilder.AppendLine("public sealed class Localizer : ILocalizer");
        stringBuilder.AppendLine("{");
        stringBuilder.AppendLine("    private readonly Locale _locale;");
        stringBuilder.AppendLine();
        stringBuilder.AppendLine("    public Localizer(Locale locale)");
        stringBuilder.AppendLine("    {");
        stringBuilder.AppendLine("        _locale = locale;");
        stringBuilder.AppendLine("    }");
        stringBuilder.AppendLine();

        foreach (var baseTranslation in localizationContext.BaseTranslation.Dictionary)
        {
            stringBuilder.AppendLine($"    public string {baseTranslation.Key}()");
            stringBuilder.AppendLine("    {");
            stringBuilder.AppendLine("        return _locale switch");
            stringBuilder.AppendLine("        {");

            foreach (var (locale, dictionary) in localizationContext.Translations)
            {
                stringBuilder.AppendLine($"            Locale.{locale} => \"{dictionary[baseTranslation.Key]}\",");
            }

            stringBuilder.AppendLine("            _ => throw new InvalidOperationException()");
            stringBuilder.AppendLine("        };");
            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine();
        }
        
        stringBuilder.AppendLine("}");

        return stringBuilder.ToString();
    }

    public const string LocalizerFactoryInterface = $$"""
                                                      {{FileHeaderComment}}
                                                      {{Namespace}}

                                                      public interface ILocalizerFactory
                                                      {
                                                          ILocalizer CreateLocalizer(Locale locale);
                                                      }
                                                      """;

    public const string LocalizerFactoryClass = $$"""
                                                  {{FileHeaderComment}}
                                                  {{Namespace}}

                                                  public sealed class LocalizerFactory : ILocalizerFactory
                                                  {
                                                      public ILocalizer CreateLocalizer(Locale locale)
                                                      {
                                                          return new Localizer(locale);
                                                      }
                                                  }
                                                  """;
}
