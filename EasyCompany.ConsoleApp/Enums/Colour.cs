using EasyCompany.SmartEnums;

namespace EasyCompany.ConsoleApp.Enums;

public sealed class Colour : SmartEnum<Colour, int>
{
    public static readonly Colour Red = new("Red", 1);
    public static readonly Colour Green = new("Green", 2);
    public static readonly Colour Blue = new("Blue", 3);

    private Colour(string name, int value) : base(name, value) { }
}