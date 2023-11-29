# TypesafeLocalization

## Usage
                          
`LocalizationConfig.json`
```json
{
  "baseLocale": "en-US"
}
```

`Translation.en-US.json`
```json
{
  "HelloWorld": "Hello world!"
}
```

`Translation.ru-RU.json`
```json
{
  "HelloWorld": "Привет мир!"
}
```

```csharp
namespace TypesafeLocalization;

public enum Locale
{
    enUS,
    ruRU
}

public interface ILocalizer
{
    string HelloWorld();
}

public sealed class Localizer : ILocalizer
{
    private readonly Locale _locale;
    
    public Localizer(Locale locale)
    {
        _locale = locale;
    }
    
    public string HelloWorld()
    {
        return _locale switch
        {
            Locale.enUS => "Hello world!",
            Locale.ruRU => "Привет мир!",
            _ => throw new InvalidOperationException()
        };
    }
}
    
public interface ILocalizerFactory
{
    ILocalizer CreateLocalizer(Locale locale);
}
    
public sealed class LocalizerFactory : ILocalizerFactory
{
    public ILocalizer CreateLocalizer(Locale locale)
    {
        return new Localizer(locale);
    }
}
```
