namespace TypesafeLocalization;

public sealed record LocalizationContext(
    LocalizationConfiguration Configuration,
    Translation BaseTranslation,
    List<Translation> Translations);

public sealed record LocalizationConfiguration(Locale BaseLocale, Strategy Strategy)
{
    public static readonly LocalizationConfiguration Default = new(new Locale("en"), Strategy.Skip);
}

public enum Strategy
{
    Skip,
    Throw
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

public sealed record Translation(string FilePath, Dictionary<string, string> Dictionary)
{
    public Locale Locale { get; } = new(Path.GetFileNameWithoutExtension(FilePath).Split('.')[1]);
}
