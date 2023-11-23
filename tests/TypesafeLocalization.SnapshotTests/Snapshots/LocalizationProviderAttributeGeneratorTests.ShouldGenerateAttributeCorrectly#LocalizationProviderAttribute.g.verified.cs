//HintName: LocalizationProviderAttribute.g.cs
namespace TypesafeLocalization;

[System.AttributeUsage(System.AttributeTargets.Assembly)]
public sealed class LocalizationProviderAttribute : System.Attribute
{
  public Locale? BaseLocale { get; set; }
}