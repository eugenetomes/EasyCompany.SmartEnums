using Newtonsoft.Json;

namespace EasyCompany.SmartEnums.NewtonsoftJson;

/// <summary>
/// A Newtonsoft.Json <see cref="JsonConverter"/> that serialises a
/// <see cref="SmartEnum{TEnum,TValue}"/> as its underlying value.
/// </summary>
/// <typeparam name="TEnum">The concrete Smart Enum type.</typeparam>
/// <typeparam name="TValue">The underlying value type.</typeparam>
public sealed class SmartEnumNewtonsoftConverter<TEnum, TValue> : JsonConverter<TEnum>
    where TEnum : SmartEnum<TEnum, TValue>
    where TValue : IEquatable<TValue>, IComparable<TValue>
{
    /// <inheritdoc/>
    public override TEnum? ReadJson(
        JsonReader reader,
        Type objectType,
        TEnum? existingValue,
        bool hasExistingValue,
        Newtonsoft.Json.JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        var value = serializer.Deserialize<TValue>(reader)
            ?? throw new Newtonsoft.Json.JsonException($"Unable to deserialise value for {typeof(TEnum).Name}.");

        try
        {
            return SmartEnum<TEnum, TValue>.FromValue(value);
        }
        catch (KeyNotFoundException ex)
        {
            throw new Newtonsoft.Json.JsonException(ex.Message, ex);
        }
    }

    /// <inheritdoc/>
    public override void WriteJson(JsonWriter writer, TEnum? value, Newtonsoft.Json.JsonSerializer serializer)
    {
        if (value is null)
        {
            writer.WriteNull();
            return;
        }

        serializer.Serialize(writer, value.Value, typeof(TValue));
    }
}
