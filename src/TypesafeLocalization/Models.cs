namespace TypesafeLocalization;

public sealed record LocalizationContext(
    Translation BaseTranslation,
    List<Translation> Translations);

public sealed record LocalizationConfiguration(Locale BaseLocale, Strategy Strategy)
{
    public static readonly LocalizationConfiguration Default = new(new Locale("en"), Strategy.Skip);
}

public enum Strategy
{
    Skip,
    Throw,
    Fallback
}

public sealed record Locale(string OriginalName)
{
    public override string ToString()
    {
        return OriginalName
            .Replace("-", "")
            .Replace("_", "");
    }
}

public sealed record Translation(string FilePath, Dictionary<string, Template> Dictionary)
{
    public Locale Locale { get; } = new(Path.GetFileNameWithoutExtension(FilePath).Split('.')[1]);
}

public sealed class Template(string text)
{
    public string Text { get; } = text;
}
