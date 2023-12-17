using Microsoft.CodeAnalysis;
using TypesafeLocalization.LightJson;
using TypesafeLocalization.LightJson.Serialization;

namespace TypesafeLocalization;

public static class AdditionalFilesParser
{
    public static LocalizationConfiguration ParseConfiguration(SourceProductionContext context, IEnumerable<AdditionalText> additionalTexts)
    {
        var additionalText = additionalTexts.FirstOrDefault();
        var jsonString = additionalText?.GetText()?.ToString();

        if (additionalText is null || jsonString is null)
        {
            return LocalizationConfiguration.Default;
        }

        try
        {
            var jsonValue = JsonValue.Parse(jsonString);
            if (!jsonValue.IsJsonObject)
            {
                return LocalizationConfiguration.Default;
            }

            var jsonObject = jsonValue.AsJsonObject;

            var baseLocale = ParseBaseLocale(jsonObject["baseLocale"]);
            var strategy = ParseStrategy(jsonObject["strategy"]);

            return new LocalizationConfiguration(baseLocale, strategy);
        }
        catch (JsonParseException exception)
        {
            var diagnostic = Diagnostics.ConfigurationFileDeserializationError(additionalText.Path, exception);
            context.ReportDiagnostic(diagnostic);
            return LocalizationConfiguration.Default;
        }

        static Locale ParseBaseLocale(JsonValue jsonValue)
        {
            return jsonValue.IsString
                ? new Locale(jsonValue.AsString)
                : LocalizationConfiguration.Default.BaseLocale;
        }

        static Strategy ParseStrategy(JsonValue jsonValue)
        {
            if (!jsonValue.IsString || !Enum.TryParse<Strategy>(jsonValue.AsString, true, out var strategy))
            {
                return LocalizationConfiguration.Default.Strategy;
            }

            return strategy;
        }
    }

    public static List<Translation> ParseTranslations(
        SourceProductionContext context,
        IEnumerable<AdditionalText> translationsTexts)
    {
        var result = new List<Translation>();

        foreach (var translationText in translationsTexts)
        {
            var jsonString = translationText.GetText()?.ToString();

            if (jsonString is null)
            {
                continue;
            }

            try
            {
                var json = JsonValue.Parse(jsonString).AsJsonObject;
                var translationDictionary = json.ToDictionary(x => new Template(x.AsString));

                var translation = new Translation(translationText.Path, translationDictionary);
                result.Add(translation);
            }
            catch (JsonParseException exception)
            {
                var diagnostic = Diagnostics.TranslationFileDeserializationError(translationText.Path, exception);
                context.ReportDiagnostic(diagnostic);
            }
        }

        return result;
    }
}
