using System.Text.RegularExpressions;

namespace Core.Helpers;

/// <summary>
/// Provides methods to sanitize JSON strings by fixing common formatting issues using precompiled regular expressions.
/// </summary>
public static partial class CoreRegex
{
    /// <summary>
    /// Sanitizes a JSON string by normalizing line endings, correcting property-value pairs, and removing unnecessary
    /// spaces and commas.
    /// </summary>
    /// <param name="sanitized">The JSON string to be sanitized. Cannot be null.</param>
    /// <returns>A sanitized JSON string with consistent formatting and spacing.</returns>
    public static string SanitizeJson(string sanitized)
    {
        sanitized = CoreRegex.NormalizeLineEndingsRegex().Replace(sanitized, "\n");
        sanitized = CoreRegex.FixPropertyValuePairsRegex1().Replace(sanitized, "\"$1\": \"");
        sanitized = CoreRegex.FixPropertyValuePairsRegex2().Replace(sanitized, "\"$1\": $2");
        sanitized = CoreRegex.FixPropertyValuePairsRegex3().Replace(sanitized, "\"$1\": $2");
        sanitized = CoreRegex.FixPropertyValuePairsRegex4().Replace(sanitized, "\"$1\": {");
        sanitized = CoreRegex.FixTrailingCommasRegex().Replace(sanitized, "$1");
        sanitized = CoreRegex.CleanUpNewlinesRegex().Replace(sanitized, "\n");
        sanitized = CoreRegex.CleanUpSpacesRegex().Replace(sanitized, " ");
        sanitized = CoreRegex.EnsureCommaSpacingRegex().Replace(sanitized, ", ");
        sanitized = CoreRegex.PreserveIndentationRegex().Replace(sanitized, "\n  $1");
        return sanitized.Replace("\\r\\n", string.Empty)
                            .Replace("\\n", string.Empty)
                            .Replace("\\\"", "\"")
                            .Replace("\r\n", string.Empty)
                            .Replace("\n", string.Empty).Trim();
    }

    /// <summary>
    /// Extracts the first URL found within the specified input string.
    /// </summary>
    /// <param name="input">The input string from which to extract the URL. Cannot be null or whitespace.</param>
    /// <returns>A string containing the first URL found in the input, or an empty string if no URL is found.</returns>
    public static string ExtractUrl(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        var match = ExtractUrlRegex().Match(input);
        return match.Success ? match.Value : string.Empty;
    }

    /// <summary>
    /// Extracts the domain name from the specified input string.
    /// </summary>
    /// <param name="input">The input string from which to extract the domain name. Cannot be null or whitespace.</param>
    /// <returns>The extracted domain name if found; otherwise, an empty string.</returns>
    public static string ExtractDomainName(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        var match = ExtractDomainNameRegex().Match(input);
        return match.Success ? match.Groups[1].Value : string.Empty;
    }

    [GeneratedRegex("https?://(?:www\\.)?([a-zA-Z0-9.-]+\\.[a-zA-Z]{2,})", RegexOptions.Compiled)]
    public static partial Regex ExtractDomainNameRegex();

    [GeneratedRegex("https?://[^\\s]+", RegexOptions.Compiled)]
    public static partial Regex ExtractUrlRegex();

    [GeneratedRegex("\\n\\s*(\\\"[^\\\"]+\\\":)", RegexOptions.Compiled)]
    public static partial Regex PreserveIndentationRegex();

    [GeneratedRegex("\\r\\n|\\r", RegexOptions.Compiled)]
    public static partial Regex NormalizeLineEndingsRegex();

    [GeneratedRegex(",\\s*[\\n\\r]*\\s*[\\]}]", RegexOptions.Compiled)]
    public static partial Regex FixTrailingCommasRegex();

    [GeneratedRegex("\\n\\s*\\n", RegexOptions.Compiled)]
    public static partial Regex CleanUpNewlinesRegex();

    [GeneratedRegex("[ \\t]+", RegexOptions.Compiled)]
    public static partial Regex CleanUpSpacesRegex();

    [GeneratedRegex(",\\s*\\n", RegexOptions.Compiled)]
    public static partial Regex EnsureCommaSpacingRegex();

    [GeneratedRegex("\"([^\"]+)\"\\s*:\\s*\\n\\s*\"", RegexOptions.Compiled)]
    public static partial Regex FixPropertyValuePairsRegex1();

    [GeneratedRegex("\"([^\"]+)\"\\s*:\\s*\\n\\s*(\\d+\\.?\\d*)", RegexOptions.Compiled)]
    public static partial Regex FixPropertyValuePairsRegex2();

    [GeneratedRegex("\"([^\"]+)\"\\s*:\\s*\\n\\s*(true|false|null)", RegexOptions.Compiled)]
    public static partial Regex FixPropertyValuePairsRegex3();

    [GeneratedRegex("\"([^\"]+)\"\\s*:\\s*\\n\\s*\\{", RegexOptions.Compiled)]
    public static partial Regex FixPropertyValuePairsRegex4();
}
