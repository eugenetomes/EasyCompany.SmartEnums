using System.Text.Json;
using EasyCompany.SmartEnums.Json;

namespace EasyCompany.SmartEnums.Tests;

public class SmartEnumJsonConverterTests
{
    private static JsonSerializerOptions BuildOptions()
        => new JsonSerializerOptions().AddSmartEnumJsonConverters();

    // ------------------------------------------------------------------
    // Serialisation (int-valued)
    // ------------------------------------------------------------------

    [Fact]
    public void Serialize_IntEnum_WritesUnderlyingValue()
    {
        var json = JsonSerializer.Serialize(Colour.Red, BuildOptions());
        Assert.Equal("1", json);
    }

    [Fact]
    public void Serialize_AllColours_WriteCorrectValues()
    {
        var options = BuildOptions();
        Assert.Equal("1", JsonSerializer.Serialize(Colour.Red,   options));
        Assert.Equal("2", JsonSerializer.Serialize(Colour.Green, options));
        Assert.Equal("3", JsonSerializer.Serialize(Colour.Blue,  options));
    }

    // ------------------------------------------------------------------
    // Deserialisation (int-valued)
    // ------------------------------------------------------------------

    [Fact]
    public void Deserialize_IntEnum_ReturnsCorrectMember()
    {
        var result = JsonSerializer.Deserialize<Colour>("2", BuildOptions());
        Assert.Equal(Colour.Green, result);
    }

    [Fact]
    public void Deserialize_UnknownIntValue_ThrowsJsonException()
    {
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<Colour>("99", BuildOptions()));
    }

    // ------------------------------------------------------------------
    // Round-trip (int-valued)
    // ------------------------------------------------------------------

    [Fact]
    public void RoundTrip_IntEnum_PreservesMember()
    {
        var options = BuildOptions();
        var json = JsonSerializer.Serialize(Colour.Blue, options);
        var result = JsonSerializer.Deserialize<Colour>(json, options);
        Assert.Equal(Colour.Blue, result);
    }

    // ------------------------------------------------------------------
    // Serialisation (string-valued)
    // ------------------------------------------------------------------

    [Fact]
    public void Serialize_StringEnum_WritesUnderlyingValue()
    {
        var json = JsonSerializer.Serialize(Direction.North, BuildOptions());
        Assert.Equal("\"N\"", json);
    }

    // ------------------------------------------------------------------
    // Deserialisation (string-valued)
    // ------------------------------------------------------------------

    [Fact]
    public void Deserialize_StringEnum_ReturnsCorrectMember()
    {
        var result = JsonSerializer.Deserialize<Direction>("\"S\"", BuildOptions());
        Assert.Equal(Direction.South, result);
    }

    // ------------------------------------------------------------------
    // Object serialisation
    // ------------------------------------------------------------------

    [Fact]
    public void Serialize_ObjectWithSmartEnumProperty_WritesUnderlyingValue()
    {
        var obj = new ColourWrapper { Colour = Colour.Green };
        var json = JsonSerializer.Serialize(obj, BuildOptions());
        Assert.Contains("\"Colour\":2", json);
    }

    [Fact]
    public void Deserialize_ObjectWithSmartEnumProperty_ReturnsCorrectMember()
    {
        var json = "{\"Colour\":3}";
        var result = JsonSerializer.Deserialize<ColourWrapper>(json, BuildOptions());
        Assert.NotNull(result);
        Assert.Equal(Colour.Blue, result!.Colour);
    }

    // ------------------------------------------------------------------
    // Factory: CanConvert
    // ------------------------------------------------------------------

    [Fact]
    public void Factory_CanConvert_ReturnsTrueForSmartEnum()
    {
        var factory = new SmartEnumJsonConverterFactory();
        Assert.True(factory.CanConvert(typeof(Colour)));
        Assert.True(factory.CanConvert(typeof(Direction)));
    }

    [Fact]
    public void Factory_CanConvert_ReturnsFalseForOtherTypes()
    {
        var factory = new SmartEnumJsonConverterFactory();
        Assert.False(factory.CanConvert(typeof(int)));
        Assert.False(factory.CanConvert(typeof(string)));
        Assert.False(factory.CanConvert(typeof(DateTime)));
    }

    // ------------------------------------------------------------------
    // Helper DTO
    // ------------------------------------------------------------------
    private sealed class ColourWrapper
    {
        public Colour Colour { get; set; } = Colour.Red;
    }
}
