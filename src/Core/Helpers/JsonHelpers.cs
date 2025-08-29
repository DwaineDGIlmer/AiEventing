using Domain.Analysis;
using System.Reflection;

namespace Core.Helpers;

/// <summary>
/// Provides methods for validating JSON strings against the structure of a specified type.
/// </summary>
/// <remarks>This class is designed to assist with strict validation of JSON input by ensuring that the
/// JSON conforms to the public instance properties of a specified type. It is particularly useful for scenarios
/// where only explicitly defined properties are allowed in the JSON data.</remarks>
public static class JsonHelpers
{
    /// <summary>
    /// Validates that the JSON string contains only properties defined in the specified type.
    /// </summary>
    /// <remarks>This method performs a strict validation of the JSON input by ensuring that all
    /// properties in the JSON object match the public instance properties of the specified type <typeparamref
    /// name="T"/>. Property name comparison is case-insensitive.</remarks>
    /// <typeparam name="T">The target type whose properties are used for validation.</typeparam>
    /// <param name="json">The JSON string to validate. Must be a valid JSON object.</param>
    /// <exception cref="JsonException">Thrown if the JSON string contains properties that are not defined in the target type <typeparamref
    /// name="T"/>.</exception>
    public static void ValidateStrict<T>(string json)
    {
        // Get all property names from the target type (case-insensitive)
        var validProps = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        try
        {
            using var doc = JsonDocument.Parse(json);
            foreach (var prop in doc.RootElement.EnumerateObject())
            {
                if (!validProps.Contains(prop.Name))
                {
                    throw new JsonException($"Unknown property '{prop.Name}' found during strict deserialization.");
                }
            }
        }
        catch (JsonException ex)
        {
            throw new JsonException("Invalid JSON format or structure.", ex);
        }
    }

    /// <summary>
    /// Parses a list of URLs and converts them into a list of <see cref="ExternalReference"/> objects.
    /// </summary>
    /// <param name="input">List of urls to return.</param>
    /// <returns>List of external references.</returns>
    public static List<ExternalReference> ParseUrls(IList<string> input)
    {
        return [.. input
                .Where(url => Uri.TryCreate(url.Trim(), UriKind.Absolute, out _))
                .Select(url => new ExternalReference
                {
                    Type = "URL",
                    Url = url.Trim(),
                    Description = $"Reference from {new Uri(url.Trim()).Host}"
                })];
    }

    /// <summary>
    /// Extracts a JSON object from the specified text.
    /// </summary>
    /// <remarks>This method searches for the first occurrence of an opening brace ('{') and the last
    /// occurrence of a closing brace ('}').  If both are found and the closing brace appears after the opening brace,
    /// the method extracts the JSON object as a substring. If the input does not contain valid braces or the braces are
    /// improperly ordered, the method returns the original input string.</remarks>
    /// <param name="text">The input string containing the JSON object. Must include both opening and closing braces for a valid
    /// extraction.</param>
    /// <returns>A substring containing the JSON object if both opening and closing braces are found in the correct order; 
    /// otherwise, returns the original input string.</returns>
    public static string ExtractJson(string text)
    {
        var start = text.IndexOf('{');
        var end = text.LastIndexOf('}');
        if (start >= 0 && end > start)
            return text.Substring(start, end - start + 1);
        return text;
    }
}
