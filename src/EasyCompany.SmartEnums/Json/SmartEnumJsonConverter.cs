using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasyCompany.SmartEnums.Json;

/// <summary>
/// A <see cref="JsonConverter{T}"/> that serialises a <see cref="SmartEnum{TEnum,TValue}"/>
/// as its underlying <typeparamref name="TValue"/> (e.g. an integer or a string).
/// </summary>
/// <typeparam name="TEnum">The concrete Smart Enum type.</typeparam>
/// <typeparam name="TValue">The underlying value type.</typeparam>
public sealed class SmartEnumJsonConverter<TEnum, TValue> : JsonConverter<TEnum>
    where TEnum : SmartEnum<TEnum, TValue>
    where TValue : IEquatable<TValue>, IComparable<TValue>
{
    private readonly JsonConverter<TValue> _valueConverter;

    /// <inheritdoc/>
    public SmartEnumJsonConverter(JsonSerializerOptions options)
    {
        _valueConverter = (JsonConverter<TValue>)(options.GetConverter(typeof(TValue))
            ?? throw new JsonException($"No JSON converter found for {typeof(TValue)}."));
    }

    /// <inheritdoc/>
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = _valueConverter.Read(ref reader, typeof(TValue), options)
            ?? throw new JsonException($"Unable to read value for {typeof(TEnum).Name}.");

        try
        {
            return SmartEnum<TEnum, TValue>.FromValue(value);
        }
        catch (KeyNotFoundException ex)
        {
            throw new JsonException(ex.Message, ex);
        }
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        => _valueConverter.Write(writer, value.Value, options);
}

/// <summary>
/// A <see cref="JsonConverterFactory"/> that creates <see cref="SmartEnumJsonConverter{TEnum,TValue}"/>
/// instances for any <see cref="SmartEnum{TEnum,TValue}"/> type automatically.
/// </summary>
public sealed class SmartEnumJsonConverterFactory : JsonConverterFactory
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsClass || typeToConvert.IsAbstract)
            return false;

        var baseType = typeToConvert.BaseType;
        while (baseType is not null)
        {
            if (baseType.IsGenericType &&
                baseType.GetGenericTypeDefinition() == typeof(SmartEnum<,>))
                return true;

            baseType = baseType.BaseType;
        }

        return false;
    }

    /// <inheritdoc/>
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var valueType = GetValueType(typeToConvert)
            ?? throw new JsonException($"Cannot determine value type for {typeToConvert.Name}.");

        var converterType = typeof(SmartEnumJsonConverter<,>).MakeGenericType(typeToConvert, valueType);
        return (JsonConverter)Activator.CreateInstance(converterType, options)!;
    }

    private static Type? GetValueType(Type enumType)
    {
        var baseType = enumType.BaseType;
        while (baseType is not null)
        {
            if (baseType.IsGenericType &&
                baseType.GetGenericTypeDefinition() == typeof(SmartEnum<,>))
                return baseType.GetGenericArguments()[1];

            baseType = baseType.BaseType;
        }

        return null;
    }
}
