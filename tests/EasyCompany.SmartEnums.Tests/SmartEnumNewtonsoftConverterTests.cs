using EasyCompany.SmartEnums.NewtonsoftJson;
using Newtonsoft.Json;

namespace EasyCompany.SmartEnums.Tests;

public class SmartEnumNewtonsoftConverterTests
{
    private static JsonSerializerSettings BuildSettings<TEnum, TValue>()
        where TEnum : SmartEnum<TEnum, TValue>
        where TValue : IEquatable<TValue>, IComparable<TValue>
        => new JsonSerializerSettings
        {
            Converters = { new SmartEnumNewtonsoftConverter<TEnum, TValue>() }
        };

    // ------------------------------------------------------------------
    // Serialisation (int-valued)
    // ------------------------------------------------------------------

    [Fact]
    public void Serialize_IntEnum_WritesUnderlyingValue()
    {
        var settings = BuildSettings<Colour, int>();
        var json = JsonConvert.SerializeObject(Colour.Red, settings);
        Assert.Equal("1", json);
    }

    // ------------------------------------------------------------------
    // Deserialisation (int-valued)
    // ------------------------------------------------------------------

    [Fact]
    public void Deserialize_IntEnum_ReturnsCorrectMember()
    {
        var settings = BuildSettings<Colour, int>();
        var result = JsonConvert.DeserializeObject<Colour>("2", settings);
        Assert.Equal(Colour.Green, result);
    }

    [Fact]
    public void Deserialize_UnknownIntValue_ThrowsJsonException()
    {
        var settings = BuildSettings<Colour, int>();
        Assert.Throws<Newtonsoft.Json.JsonException>(() =>
            JsonConvert.DeserializeObject<Colour>("99", settings));
    }

    // ------------------------------------------------------------------
    // Round-trip (int-valued)
    // ------------------------------------------------------------------

    [Fact]
    public void RoundTrip_IntEnum_PreservesMember()
    {
        var settings = BuildSettings<Colour, int>();
        var json = JsonConvert.SerializeObject(Colour.Blue, settings);
        var result = JsonConvert.DeserializeObject<Colour>(json, settings);
        Assert.Equal(Colour.Blue, result);
    }

    // ------------------------------------------------------------------
    // Serialisation (string-valued)
    // ------------------------------------------------------------------

    [Fact]
    public void Serialize_StringEnum_WritesUnderlyingValue()
    {
        var settings = BuildSettings<Direction, string>();
        var json = JsonConvert.SerializeObject(Direction.North, settings);
        Assert.Equal("\"N\"", json);
    }

    // ------------------------------------------------------------------
    // Deserialisation (string-valued)
    // ------------------------------------------------------------------

    [Fact]
    public void Deserialize_StringEnum_ReturnsCorrectMember()
    {
        var settings = BuildSettings<Direction, string>();
        var result = JsonConvert.DeserializeObject<Direction>("\"S\"", settings);
        Assert.Equal(Direction.South, result);
    }

    // ------------------------------------------------------------------
    // Null handling
    // ------------------------------------------------------------------

    [Fact]
    public void Deserialize_Null_ReturnsNull()
    {
        var settings = BuildSettings<Colour, int>();
        var result = JsonConvert.DeserializeObject<Colour?>("null", settings);
        Assert.Null(result);
    }

    [Fact]
    public void Serialize_Null_WritesNull()
    {
        var settings = BuildSettings<Colour, int>();
        var json = JsonConvert.SerializeObject(null, typeof(Colour), settings);
        Assert.Equal("null", json);
    }
}
