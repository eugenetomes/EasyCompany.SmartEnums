# EasyCompany.SmartEnums

A lightweight Smart Enum base-class library for .NET 8+ that goes beyond plain C# enums by providing rich behaviour: discovery, parsing, equality, comparison, and first-class JSON serialisation support for both **System.Text.Json** and **Newtonsoft.Json**.

---

## Features

- **`SmartEnum<TEnum, TValue>`** abstract base class
- Any comparable value type: `int`, `string`, `Guid`, …
- `List` / `GetAll()` – enumerate all declared members at runtime
- `FromName()` / `TryFromName()` – parse by display name (case-sensitive or case-insensitive)
- `FromValue()` / `TryFromValue()` – parse by underlying value
- Equality (`==`, `!=`) and comparison (`<`, `>`, `<=`, `>=`) operators
- Implicit conversion to the underlying value type
- **System.Text.Json** `SmartEnumJsonConverterFactory` + `AddSmartEnumJsonConverters()` extension
- **Newtonsoft.Json** `SmartEnumNewtonsoftConverter<TEnum, TValue>` converter

---

## Installation

```bash
dotnet add package EasyCompany.SmartEnums
```

---

## Quick Start

### 1 – Define a Smart Enum

```csharp
using EasyCompany.SmartEnums;

public sealed class Colour : SmartEnum<Colour, int>
{
    public static readonly Colour Red   = new("Red",   1);
    public static readonly Colour Green = new("Green", 2);
    public static readonly Colour Blue  = new("Blue",  3);

    private Colour(string name, int value) : base(name, value) { }
}
```

You can use **any comparable value type**; here is a string-valued example:

```csharp
public sealed class Direction : SmartEnum<Direction, string>
{
    public static readonly Direction North = new("North", "N");
    public static readonly Direction South = new("South", "S");
    public static readonly Direction East  = new("East",  "E");
    public static readonly Direction West  = new("West",  "W");

    private Direction(string name, string value) : base(name, value) { }
}
```

### 2 – Use it

```csharp
// Enumerate all members
foreach (var colour in Colour.List)
    Console.WriteLine($"{colour.Name} = {colour.Value}");

// Parse by name
var red   = Colour.FromName("Red");
var blue  = Colour.FromName("blue", ignoreCase: true);

// Parse by value
var green = Colour.FromValue(2);

// Try-parse variants
if (Colour.TryFromName("Purple", out var unknown))
    Console.WriteLine(unknown!.Name);

// Operators
Console.WriteLine(Colour.Red == Colour.Red);   // True
Console.WriteLine(Colour.Red <  Colour.Blue);  // True

// Implicit conversion
int value = Colour.Green;   // 2
```

### 3 – JSON serialisation (System.Text.Json)

```csharp
using EasyCompany.SmartEnums.Json;
using System.Text.Json;

var options = new JsonSerializerOptions().AddSmartEnumJsonConverters();

string json   = JsonSerializer.Serialize(Colour.Red, options);   // "1"
Colour colour = JsonSerializer.Deserialize<Colour>("2", options)!; // Green

// Works inside object graphs too
public record Order(int Id, Colour Status);

var order = new Order(42, Colour.Blue);
string orderJson = JsonSerializer.Serialize(order, options);
// {"Id":42,"Status":3}
```

### 4 – JSON serialisation (Newtonsoft.Json)

```csharp
using EasyCompany.SmartEnums.NewtonsoftJson;
using Newtonsoft.Json;

var settings = new JsonSerializerSettings
{
    Converters = { new SmartEnumNewtonsoftConverter<Colour, int>() }
};

string json   = JsonConvert.SerializeObject(Colour.Red, settings);    // "1"
Colour colour = JsonConvert.DeserializeObject<Colour>("2", settings)!; // Green
```

---

## API Reference

| Member | Description |
|---|---|
| `Name` | Display name of the member |
| `Value` | Underlying typed value |
| `List` | Read-only list of all declared members |
| `GetAll()` | Same as `List` |
| `FromName(name, ignoreCase?)` | Returns member or throws `KeyNotFoundException` |
| `TryFromName(name, out result, ignoreCase?)` | Returns `bool`; `result` is `null` on miss |
| `FromValue(value)` | Returns member or throws `KeyNotFoundException` |
| `TryFromValue(value, out result)` | Returns `bool`; `result` is `null` on miss |
| `ToString()` | Returns `Name` |
| `implicit operator TValue` | Converts to underlying value without a cast |

---

## Optional: EF Core Value Converter
```csharp
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class SmartEnumConverter<TEnum, TValue>
    : ValueConverter<TEnum, TValue>
    where TEnum : SmartEnum<TEnum, TValue>
    where TValue : IEquatable<TValue>, IComparable<TValue>
{
    public SmartEnumConverter()
        : base(
            v => v.Value,
            v => SmartEnum<TEnum, TValue>.FromValue(v))
    { }
}

```

Usage
```csharp
builder.Property(x => x.Status)
       .HasConversion(new SmartEnumConverter<OrderStatus, int>());
```

## License

MIT – see [LICENSE](LICENSE).
