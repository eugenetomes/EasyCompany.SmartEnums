using System.Reflection;

namespace EasyCompany.SmartEnums;

/// <summary>
/// Abstract base class for creating Smart Enums.
/// </summary>
/// <typeparam name="TEnum">The concrete Smart Enum type that derives from this class.</typeparam>
/// <typeparam name="TValue">The underlying value type (e.g. <see cref="int"/>, <see cref="string"/>).</typeparam>
public abstract class SmartEnum<TEnum, TValue>
    : IEquatable<SmartEnum<TEnum, TValue>>,
      IComparable<SmartEnum<TEnum, TValue>>
    where TEnum : SmartEnum<TEnum, TValue>
    where TValue : IEquatable<TValue>, IComparable<TValue>
{
    // -------------------------------------------------------------------------
    // Static registry of all members for this concrete enum type
    // -------------------------------------------------------------------------
    private static readonly Lazy<IReadOnlyList<TEnum>> _allMembers = new(LoadMembers, isThreadSafe: true);

    private static IReadOnlyList<TEnum> LoadMembers()
    {
        var enumType = typeof(TEnum);
        var members = new List<TEnum>();

        // Collect every public static field / property of the concrete type
        // that is an instance of TEnum.
        foreach (var field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
        {
            if (field.FieldType == enumType && field.GetValue(null) is TEnum member)
                members.Add(member);
        }

        foreach (var prop in enumType.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
        {
            if (prop.PropertyType == enumType && prop.GetValue(null) is TEnum member)
                members.Add(member);
        }

        return members.AsReadOnly();
    }

    // -------------------------------------------------------------------------
    // Instance members
    // -------------------------------------------------------------------------

    /// <summary>Gets the display name of this Smart Enum member.</summary>
    public string Name { get; }

    /// <summary>Gets the underlying value of this Smart Enum member.</summary>
    public TValue Value { get; }

    /// <summary>
    /// Initialises a new Smart Enum member.
    /// </summary>
    /// <param name="name">The display name.</param>
    /// <param name="value">The underlying value.</param>
    protected SmartEnum(string name, TValue value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(value);

        Name = name;
        Value = value;
    }

    // -------------------------------------------------------------------------
    // Static helpers
    // -------------------------------------------------------------------------

    /// <summary>Returns a read-only list of all declared members of this Smart Enum.</summary>
    public static IReadOnlyList<TEnum> List => _allMembers.Value;

    /// <summary>Returns a read-only list of all declared members of this Smart Enum.</summary>
    public static IReadOnlyList<TEnum> GetAll() => _allMembers.Value;

    /// <summary>
    /// Retrieves a member by its <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name to look up (case-sensitive by default).</param>
    /// <param name="ignoreCase">When <see langword="true"/> the comparison is case-insensitive.</param>
    /// <returns>The matching Smart Enum member.</returns>
    /// <exception cref="KeyNotFoundException">No member with the given name exists.</exception>
    public static TEnum FromName(string name, bool ignoreCase = false)
    {
        var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        var match = _allMembers.Value.FirstOrDefault(m => string.Equals(m.Name, name, comparison));

        return match ?? throw new KeyNotFoundException(
            $"No {typeof(TEnum).Name} member with name '{name}' was found.");
    }

    /// <summary>
    /// Tries to retrieve a member by its <paramref name="name"/>.
    /// </summary>
    /// <returns><see langword="true"/> if a match was found; otherwise <see langword="false"/>.</returns>
    public static bool TryFromName(string name, out TEnum? result, bool ignoreCase = false)
    {
        var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        result = _allMembers.Value.FirstOrDefault(m => string.Equals(m.Name, name, comparison));
        return result is not null;
    }

    /// <summary>
    /// Retrieves a member by its underlying <paramref name="value"/>.
    /// </summary>
    /// <returns>The matching Smart Enum member.</returns>
    /// <exception cref="KeyNotFoundException">No member with the given value exists.</exception>
    public static TEnum FromValue(TValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var match = _allMembers.Value.FirstOrDefault(m => m.Value.Equals(value));

        return match ?? throw new KeyNotFoundException(
            $"No {typeof(TEnum).Name} member with value '{value}' was found.");
    }

    /// <summary>
    /// Tries to retrieve a member by its underlying <paramref name="value"/>.
    /// </summary>
    /// <returns><see langword="true"/> if a match was found; otherwise <see langword="false"/>.</returns>
    public static bool TryFromValue(TValue value, out TEnum? result)
    {
        result = value is null ? null : _allMembers.Value.FirstOrDefault(m => m.Value.Equals(value));
        return result is not null;
    }

    // -------------------------------------------------------------------------
    // Equality / comparison
    // -------------------------------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(SmartEnum<TEnum, TValue>? other)
        => other is not null && Value.Equals(other.Value);

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is SmartEnum<TEnum, TValue> other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => Value.GetHashCode();

    /// <inheritdoc/>
    public int CompareTo(SmartEnum<TEnum, TValue>? other)
        => other is null ? 1 : Value.CompareTo(other.Value);

    /// <inheritdoc/>
    public override string ToString() => Name;

    // -------------------------------------------------------------------------
    // Operators
    // -------------------------------------------------------------------------

    /// <summary>Implicit conversion to the underlying value type.</summary>
    public static implicit operator TValue(SmartEnum<TEnum, TValue> smartEnum)
    {
        ArgumentNullException.ThrowIfNull(smartEnum);
        return smartEnum.Value;
    }

    /// <summary>Equality operator.</summary>
    public static bool operator ==(SmartEnum<TEnum, TValue>? left, SmartEnum<TEnum, TValue>? right)
        => left is null ? right is null : left.Equals(right);

    /// <summary>Inequality operator.</summary>
    public static bool operator !=(SmartEnum<TEnum, TValue>? left, SmartEnum<TEnum, TValue>? right)
        => !(left == right);

    /// <summary>Less-than operator.</summary>
    public static bool operator <(SmartEnum<TEnum, TValue> left, SmartEnum<TEnum, TValue> right)
        => left.CompareTo(right) < 0;

    /// <summary>Greater-than operator.</summary>
    public static bool operator >(SmartEnum<TEnum, TValue> left, SmartEnum<TEnum, TValue> right)
        => left.CompareTo(right) > 0;

    /// <summary>Less-than-or-equal operator.</summary>
    public static bool operator <=(SmartEnum<TEnum, TValue> left, SmartEnum<TEnum, TValue> right)
        => left.CompareTo(right) <= 0;

    /// <summary>Greater-than-or-equal operator.</summary>
    public static bool operator >=(SmartEnum<TEnum, TValue> left, SmartEnum<TEnum, TValue> right)
        => left.CompareTo(right) >= 0;
}
