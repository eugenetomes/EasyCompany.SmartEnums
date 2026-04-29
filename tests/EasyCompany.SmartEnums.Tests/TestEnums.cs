namespace EasyCompany.SmartEnums.Tests;

/// <summary>
/// A sample Smart Enum used across the test suite.
/// </summary>
public sealed class Colour : SmartEnum<Colour, int>
{
    public static readonly Colour Red   = new("Red",   1);
    public static readonly Colour Green = new("Green", 2);
    public static readonly Colour Blue  = new("Blue",  3);

    private Colour(string name, int value) : base(name, value) { }
}

/// <summary>
/// A sample string-valued Smart Enum.
/// </summary>
public sealed class Direction : SmartEnum<Direction, string>
{
    public static readonly Direction North = new("North", "N");
    public static readonly Direction South = new("South", "S");
    public static readonly Direction East  = new("East",  "E");
    public static readonly Direction West  = new("West",  "W");

    private Direction(string name, string value) : base(name, value) { }
}
