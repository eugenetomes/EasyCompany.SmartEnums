using System.Text.Json;

namespace EasyCompany.SmartEnums.Json;

/// <summary>
/// Extension methods for configuring <see cref="JsonSerializerOptions"/> with Smart Enum support.
/// </summary>
public static class SmartEnumJsonExtensions
{
    /// <summary>
    /// Registers the <see cref="SmartEnumJsonConverterFactory"/> so that all
    /// <see cref="SmartEnum{TEnum,TValue}"/> types are automatically serialised /
    /// deserialised using their underlying value.
    /// </summary>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to configure.</param>
    /// <returns>The same <paramref name="options"/> instance for fluent chaining.</returns>
    public static JsonSerializerOptions AddSmartEnumJsonConverters(this JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.Converters.Add(new SmartEnumJsonConverterFactory());
        return options;
    }
}
