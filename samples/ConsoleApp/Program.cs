using TypesafeLocalization;

var localizerEn = new Localizer(Locale.enUS);
var localizerRu = new Localizer(Locale.ruRU);

Console.WriteLine(localizerEn.Key1());
Console.WriteLine(localizerEn.Key2());
Console.WriteLine(localizerEn.Key3());

Console.WriteLine(localizerRu.Key1());
Console.WriteLine(localizerRu.Key2());
Console.WriteLine(localizerRu.Key3());
