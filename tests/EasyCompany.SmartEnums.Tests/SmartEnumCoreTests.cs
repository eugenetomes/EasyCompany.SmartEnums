namespace EasyCompany.SmartEnums.Tests;

public class SmartEnumCoreTests
{
    // ------------------------------------------------------------------
    // List / GetAll
    // ------------------------------------------------------------------

    [Fact]
    public void List_ReturnsAllDeclaredMembers()
    {
        var all = Colour.List;
        Assert.Equal(3, all.Count);
        Assert.Contains(Colour.Red,   all);
        Assert.Contains(Colour.Green, all);
        Assert.Contains(Colour.Blue,  all);
    }

    [Fact]
    public void GetAll_ReturnsSameAsListProperty()
    {
        Assert.Equal(Colour.List, Colour.GetAll());
    }

    // ------------------------------------------------------------------
    // Name / Value properties
    // ------------------------------------------------------------------

    [Fact]
    public void Name_ReturnsConfiguredName()
    {
        Assert.Equal("Red",   Colour.Red.Name);
        Assert.Equal("Green", Colour.Green.Name);
    }

    [Fact]
    public void Value_ReturnsConfiguredValue()
    {
        Assert.Equal(1, Colour.Red.Value);
        Assert.Equal(2, Colour.Green.Value);
    }

    [Fact]
    public void ToString_ReturnsName()
    {
        Assert.Equal("Red",   Colour.Red.ToString());
        Assert.Equal("North", Direction.North.ToString());
    }

    // ------------------------------------------------------------------
    // FromName
    // ------------------------------------------------------------------

    [Fact]
    public void FromName_CaseSensitive_ReturnsCorrectMember()
    {
        var result = Colour.FromName("Blue");
        Assert.Equal(Colour.Blue, result);
    }

    [Fact]
    public void FromName_CaseInsensitive_ReturnsCorrectMember()
    {
        var result = Colour.FromName("blue", ignoreCase: true);
        Assert.Equal(Colour.Blue, result);
    }

    [Fact]
    public void FromName_UnknownName_ThrowsKeyNotFoundException()
    {
        Assert.Throws<KeyNotFoundException>(() => Colour.FromName("Purple"));
    }

    [Fact]
    public void TryFromName_ExistingName_ReturnsTrueAndMember()
    {
        var found = Colour.TryFromName("Red", out var result);
        Assert.True(found);
        Assert.Equal(Colour.Red, result);
    }

    [Fact]
    public void TryFromName_UnknownName_ReturnsFalse()
    {
        var found = Colour.TryFromName("Purple", out var result);
        Assert.False(found);
        Assert.Null(result);
    }

    // ------------------------------------------------------------------
    // FromValue
    // ------------------------------------------------------------------

    [Fact]
    public void FromValue_KnownValue_ReturnsCorrectMember()
    {
        var result = Colour.FromValue(2);
        Assert.Equal(Colour.Green, result);
    }

    [Fact]
    public void FromValue_UnknownValue_ThrowsKeyNotFoundException()
    {
        Assert.Throws<KeyNotFoundException>(() => Colour.FromValue(99));
    }

    [Fact]
    public void TryFromValue_ExistingValue_ReturnsTrueAndMember()
    {
        var found = Colour.TryFromValue(1, out var result);
        Assert.True(found);
        Assert.Equal(Colour.Red, result);
    }

    [Fact]
    public void TryFromValue_UnknownValue_ReturnsFalse()
    {
        var found = Colour.TryFromValue(99, out var result);
        Assert.False(found);
        Assert.Null(result);
    }

    // ------------------------------------------------------------------
    // String-valued enum
    // ------------------------------------------------------------------

    [Fact]
    public void StringEnum_FromValue_ReturnsCorrectMember()
    {
        var result = Direction.FromValue("N");
        Assert.Equal(Direction.North, result);
    }

    [Fact]
    public void StringEnum_FromName_ReturnsCorrectMember()
    {
        var result = Direction.FromName("South");
        Assert.Equal(Direction.South, result);
    }

    // ------------------------------------------------------------------
    // Equality
    // ------------------------------------------------------------------

    [Fact]
    public void SameMember_IsEqual()
    {
        var red1 = Colour.Red;
        var red2 = Colour.Red;
        Assert.Equal(red1, red2);
        Assert.True(red1 == red2);
        Assert.False(red1 != red2);
    }

    [Fact]
    public void DifferentMembers_AreNotEqual()
    {
        Assert.NotEqual(Colour.Red, Colour.Blue);
        Assert.True(Colour.Red != Colour.Blue);
    }

    // ------------------------------------------------------------------
    // Comparison
    // ------------------------------------------------------------------

    [Fact]
    public void LessThanOperator_Works()
    {
        Assert.True(Colour.Red < Colour.Green);
        Assert.False(Colour.Blue < Colour.Red);
    }

    [Fact]
    public void GreaterThanOperator_Works()
    {
        Assert.True(Colour.Blue > Colour.Green);
        Assert.False(Colour.Red > Colour.Blue);
    }

    // ------------------------------------------------------------------
    // Implicit conversion
    // ------------------------------------------------------------------

    [Fact]
    public void ImplicitConversion_ToValue_Works()
    {
        int value = Colour.Red;
        Assert.Equal(1, value);
    }

    // ------------------------------------------------------------------
    // Guard clauses
    // ------------------------------------------------------------------

    [Fact]
    public void FromName_WhitespaceName_ThrowsKeyNotFoundException()
    {
        // Whitespace is a valid search string but matches no member
        Assert.Throws<KeyNotFoundException>(() => Colour.FromName("  "));
    }
}
