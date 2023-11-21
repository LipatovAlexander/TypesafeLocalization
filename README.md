# TypesafeLocalization

## Usage

```json
{
  "HelloWorld": "Hello, world!",
  "Key": "{{what}} is {{how}}",
  "Key2": "There is {{count:int}} items"
}
```

```csharp
public interface ILocalizer
{
  public string HelloWorld();
  public string Key(string what, string how);
  public string Key2(int count);
}
```

## Compile-time warnings

Основной файл локализации (`*.en.json`):
```json
{
  "HelloWorld": "Hello, world!",
  "Greeting": "Hello, {{name}}"
}
```

Второстепенный файл локализации (`*.ru.json`):
```json
{
  "GoodbyeWorld": "Goodbye, world!", // Warning: Неизвестный ключ GoodbyeWorld
  "Greeting": "Hello, {{user}}" // Warning: Неизвестный параметр user
} // Warning: Отсутствует ключ HelloWorld
```