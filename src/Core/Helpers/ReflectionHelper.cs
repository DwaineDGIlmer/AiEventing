using System.Reflection;

namespace Core.Helpers;

/// <summary>
/// Provides utility methods for reflection-based operations.
/// </summary>
/// <remarks>This class includes methods to assist with reflection tasks, such as determining whether a
/// property value should be ignored during updates based on its type and value.</remarks>
public class ReflectionHelper
{
    /// <summary>
    /// Determines whether a property value should be ignored during updates.
    /// </summary>
    /// <param name="property">The property being evaluated.</param>
    /// <param name="value">The incoming value for the property.</param>
    /// <returns>True if the property should be ignored; otherwise, false.</returns>
    public static bool ShouldIgnoreProperty(PropertyInfo property, object? value)
    {
        var propertyType = property.PropertyType;
        var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        // Handle null values
        if (value == null)
            return true;

        // Handle string properties
        if (underlyingType == typeof(string))
        {
            return string.IsNullOrWhiteSpace(value.ToString());
        }

        // Handle DateTime properties
        if (underlyingType == typeof(DateTime))
        {
            var dateValue = (DateTime)value;
            return dateValue == default || dateValue == DateTime.MinValue;
        }

        // Handle collections (List, Array, etc.)
        if (value is System.Collections.IEnumerable enumerable &&
            underlyingType != typeof(string)) // string is IEnumerable but we handle it separately
        {
            var enumerator = enumerable.GetEnumerator();
            bool hasItems = enumerator.MoveNext();
            if (enumerator is IDisposable disposable)
                disposable.Dispose();
            return !hasItems;
        }

        // Handle numeric types (int, decimal, double, etc.)
        if (underlyingType.IsPrimitive || underlyingType == typeof(decimal))
        {
            // For numeric types, we generally don't ignore unless it's a special case
            return false;
        }

        // Handle boolean - don't ignore boolean values as false is a valid state
        if (underlyingType == typeof(bool))
        {
            return false;
        }

        // Handle enum types
        if (underlyingType.IsEnum)
        {
            // Don't ignore enum values unless you have specific business logic
            return false;
        }

        // For nullable types, check if the value is null
        if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return value == null;
        }

        // Default: don't ignore other types
        return false;
    }
}
