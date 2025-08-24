using Core.Serializers;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

[assembly: InternalsVisibleTo("UnitTests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c599f69c3dd3ec398aa236557324d13db0f01fe1619e95bd66ab4fbd53143b2e57470a9c156080f2e3b088da0a7d40ce549ed7d803bc7cfc904077dce8ea5262c4afc77594841fb916db84485db81dfa6ba6cba1d449c0cb8c6aafd42245221dc310fae03f9b18c258c7939cd293f01a9b1ab9c433a53b278022f02a46958797")]
namespace Core.Extensions;

/// <summary>
/// Provides extension methods for various utility operations.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Generates a SHA256-based hash for the specified object and returns a ulong.
    /// </summary>
    /// <typeparam name="T">The type of the object to hash.</typeparam>
    /// <param name="obj">The object to generate the hash for. Cannot be <see langword="null"/>.</param>
    /// <returns>A ulong representing the SHA256 hash of the serialized object.</returns>
    public static ulong GenHash<T>(this T obj)
    {
        if (obj == null)
            return 0UL;

        try
        {
            // Serialize messages to JSON
            var serializedMessages = GenHashString<T>(obj);

            // Compute SHA256 hash for uniqueness using HashData
            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(serializedMessages));

            // SHA256 always returns 32 bytes, so this is safe
            return BitConverter.ToUInt64(hashBytes, 0);
        }
        catch
        {
            return 0UL;
        }
    }

    /// <summary>
    /// Generates a SHA256-based hash for the specified object.
    /// </summary>
    /// <typeparam name="T">The type of the object to hash.</typeparam>
    /// <param name="obj">The object to generate the hash for. Cannot be <see langword="null"/>.</param>
    /// <returns>A Base64-encoded string representing the SHA256 hash of the serialized object.</returns>
    public static string GenHashString<T>(this T obj)
    {
        if (obj == null)
            return string.Empty;

        // Serialize messages to JSON
        var serializedMessages = JsonConvertService.Instance.Serialize(obj);

        // Compute SHA256 hash for uniqueness using HashData
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(serializedMessages));

        // Convert hash bytes to a Base64 string
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

    /// <summary>
    /// Sends a JSON-encoded object to the specified URI using an HTTP POST request.
    /// </summary>
    /// <remarks>This method uses the specified <see cref="IHttpClientFactory"/> to create an HTTP client and
    /// send a POST request. If an exception occurs during the operation, the error is logged to the console, and <see
    /// langword="null"/> is returned.</remarks>
    /// <typeparam name="T">The type of the object to be serialized and sent in the request body.</typeparam>
    /// <param name="factory">The <see cref="IHttpClientFactory"/> used to create the HTTP client.</param>
    /// <param name="httpClientName">The name of the HTTP client to use. This must match a client configuration registered with the factory.</param>
    /// <param name="requestUri">The URI to which the request is sent. This cannot be null or empty.</param>
    /// <param name="keyValue">A key-value pair to be added to the request headers. This can be used for custom headers or authentication.</param>
    /// <param name="value">The object to be serialized as JSON and sent in the request body. This cannot be null.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation. The result is an <see
    /// cref="HttpResponseMessage"/> if the request completes successfully; otherwise, <see langword="null"/> is
    /// returned.</returns>
    public async static Task<HttpResponseMessage?> PostAsJsonAsync<T>(
        this IHttpClientFactory factory,
        string httpClientName,
        string requestUri,
        KeyValuePair<string, string>? keyValue,
        T value,
        CancellationToken cancellationToken)
    {
        factory.IsNullThrow(nameof(factory));
        httpClientName.IsNullThrow(nameof(httpClientName));
        requestUri.IsNullThrow(nameof(requestUri));
        value.IsNullThrow(nameof(value));

        try
        {
            var client = factory.CreateClient(httpClientName);
            if (keyValue.HasValue && !string.IsNullOrEmpty(keyValue.Value.Key) && !string.IsNullOrEmpty(keyValue.Value.Value))
            {
                client.DefaultRequestHeaders.Add(keyValue.Value.Key, keyValue.Value.Value);
            }
            return await client.PostAsJsonAsync(requestUri, value, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error posting JSON data: {ex.Message}");
        }
        return null;
    }

    /// <summary>
    /// Sends a GET request to the specified URI using a named HTTP client and retrieves the response body as a string.
    /// </summary>
    /// <remarks>This method uses the named HTTP client specified by <paramref name="httpClientName"/> to send
    /// the request.  If an exception occurs during the request, the method logs the error and returns <see
    /// langword="null"/>.</remarks>
    /// <param name="factory">The <see cref="IHttpClientFactory"/> used to create the named HTTP client.</param>
    /// <param name="httpClientName">The name of the HTTP client to use. This must match a client configured in the application's dependency
    /// injection container.</param>
    /// <param name="requestUri">The URI of the resource to retrieve.</param>
    /// <param name="keyValue">A key-value pair to be added to the request headers. This can be used for custom headers or authentication.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the response body as a string,  or
    /// <see langword="null"/> if an error occurs during the request.</returns>
    public async static Task<string?> GetStringAsync(
        this IHttpClientFactory factory,
        string httpClientName,
        string requestUri,
        KeyValuePair<string, string>? keyValue,
        CancellationToken cancellationToken)
    {
        factory.IsNullThrow(nameof(factory));
        httpClientName.IsNullThrow(nameof(httpClientName));
        requestUri.IsNullThrow(nameof(requestUri));

        try
        {
            var client = factory.CreateClient(httpClientName);
            if (keyValue.HasValue && !string.IsNullOrEmpty(keyValue.Value.Key) && !string.IsNullOrEmpty(keyValue.Value.Value))
            {
                client.DefaultRequestHeaders.Add(keyValue.Value.Key, keyValue.Value.Value);
            }
            return await client.GetStringAsync(requestUri, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error getting string from URI: {ex.Message}");
        }
        return null;
    }

    /// <summary>
    /// Sends a GET request to the specified URI using a named HTTP client and retrieves the response body as a string.
    /// </summary>
    /// <remarks>This method uses the named HTTP client specified by <paramref name="httpClientName"/> to send
    /// the request.  If an exception occurs during the request, the method logs the error and returns <see
    /// langword="null"/>.</remarks>
    /// <param name="factory">The <see cref="IHttpClientFactory"/> used to create the named HTTP client.</param>
    /// <param name="httpClientName">The name of the HTTP client to use. This must match a client configured in the application's dependency
    /// injection container.</param>
    /// <param name="requestUri">The URI of the resource to retrieve.</param>
    /// <param name="keyValue">A key-value pair to be added to the request headers. This can be used for custom headers or authentication.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the response body as a string,  or
    /// <see langword="null"/> if an error occurs during the request.</returns>
    public async static Task<HttpResponseMessage?> GetAsync(
        this IHttpClientFactory factory,
        string httpClientName,
        string requestUri,
        KeyValuePair<string, string>? keyValue,
        CancellationToken cancellationToken)
    {
        factory.IsNullThrow(nameof(factory));
        httpClientName.IsNullThrow(nameof(httpClientName));
        requestUri.IsNullThrow(nameof(requestUri));

        try
        {
            var client = factory.CreateClient(httpClientName);
            if (keyValue.HasValue && !string.IsNullOrEmpty(keyValue.Value.Key) && !string.IsNullOrEmpty(keyValue.Value.Value))
            {
                client.DefaultRequestHeaders.Add(keyValue.Value.Key, keyValue.Value.Value);
            }
            return await client.GetAsync(requestUri, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error getting string from URI: {ex.Message}");
        }
        return null;
    }

    /// <summary>
    /// Converts the specified object to its JSON string representation.
    /// </summary>
    /// <remarks>This method uses the <see cref="JsonConvertService"/> to perform serialization. Ensure that
    /// the object being serialized is compatible with the JSON serialization process.</remarks>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="obj">The object to serialize. Cannot be <see langword="null"/>.</param>
    /// <returns>A JSON string representation of the specified object.</returns>
    /// <exception cref="JsonException">When serializtion fails this exception is thrown.</exception>
    public static string ToJson<T>(this T obj)
    {
        return JsonConvertService.Instance.Serialize(obj);
    }

    /// <summary>
    /// Tries to converts the specified object to its JSON string representation.
    /// </summary>
    /// <remarks>This method uses the <see cref="JsonConvertService"/> to perform serialization. Ensure that
    /// the object being serialized is compatible with the JSON serialization process.</remarks>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="obj">The object to serialize. Cannot be <see langword="null"/>.</param>
    /// <returns>A JSON string representation of the specified object.</returns>
    public static string ToJsonTry<T>(this T obj)
    {
        try
        {
            return JsonConvertService.Instance.Serialize(obj);
        }
        catch (JsonException ex)
        {
            Console.Error.WriteLine($"Error serializing object to JSON: {ex.Message}");
        }
        return string.Empty;
    }

    /// <summary>
    /// Attempts to serialize the specified object to a JSON string using the provided serialization options.
    /// </summary>
    /// <remarks>If serialization fails due to a <see cref="JsonException"/>, an error message is logged to
    /// the standard error output.</remarks>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="obj">The object to serialize to JSON. Cannot be <see langword="null"/>.</param>
    /// <param name="options">The options to use for JSON serialization. If <see langword="null"/>, default options are applied.</param>
    /// <returns>A JSON string representation of the object if serialization is successful; otherwise, an empty string.</returns>
    public static string ToJsonTry<T>(this T obj, JsonSerializerOptions options)
    {
        try
        {
            return JsonConvertService.Instance.Serialize(obj, options);
        }
        catch (JsonException ex)
        {
            Console.Error.WriteLine($"Error serializing object to JSON: {ex.Message}");
        }
        return string.Empty;
    }

    /// <summary>
    /// Deserilizes the specified JSON string into an object of type <typeparamref name="T"/>.  
    /// </summary>
    /// <param name="obj">The JSON string to deserialize. Cannot be <see langword="null"/>.</param>
    /// <exception cref="JsonException">When serializtion fails this exception is thrown.</exception>
    public static T ToObject<T>(this string obj)
    {
        return JsonConvertService.Instance.Deserialize<T>(obj);
    }

    /// <summary>
    /// Tries to deserilizes the specified JSON string into an object of type <typeparamref name="T"/>.  
    /// </summary>
    /// <param name="obj">The JSON string to deserialize. Cannot be <see langword="null"/>.</param>
    public static T? ToObjectTry<T>(this string obj)
    {
        try
        {
            return JsonConvertService.Instance.Deserialize<T>(obj);
        }
        catch (JsonException ex)
        {
            Console.Error.WriteLine($"Error serializing object to JSON: {ex.Message}");
        }
        return default;
    }

    /// <summary>
    /// Determines whether the specified object is null.
    /// </summary>
    /// <typeparam name="T">The type of the object to check.</typeparam>
    /// <param name="obj">The object to check for null.</param>
    /// <returns><c>true</c> if the object is null; otherwise, <c>false</c>.</returns>
    public static bool IsNull<T>([NotNullWhen(false)] this T obj)
    {
        if (obj is string str)
            return string.IsNullOrEmpty(str);

        return obj is null;
    }

    /// <summary>
    /// Ensures that the specified object is not null or, in the case of a string, not empty.  Throws an exception if
    /// the validation fails.
    /// </summary>
    /// <typeparam name="T">The type of the object being validated.</typeparam>
    /// <param name="obj">The object to validate. If the object is a string, it must not be null or empty.</param>
    /// <param name="message">An optional custom error message to include in the exception. If not provided, a default message is used.</param>
    /// <param name="paramName">The name of the parameter being validated. This is automatically captured by the compiler.</param>
    /// <returns>The validated object, if it is not null or empty.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null, or if <paramref name="obj"/> is a string and is empty.</exception>
    public static T IsNullThrow<T>(
        [NotNullWhen(false)] this T obj,
        string? message = null,
        [CallerArgumentExpression(nameof(obj))] string? paramName = null)
    {
        bool isNull = false;
        if (obj is null)
        {
            isNull = true;
        }
        else if (obj is string str)
        {
            isNull = string.IsNullOrEmpty(str);
        }

        if (isNull)
            throw new ArgumentNullException(paramName, message ?? "Object cannot be null or empty.");
        return obj;
    }

    /// <summary>
    /// Ensures that the specified IList is not null or empty. Throws an <see cref="ArgumentNullException"/> if the list is null or empty.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the list.</typeparam>
    /// <param name="list">The list to validate.</param>
    /// <param name="message">An optional message to include in the exception. If not provided, a default message is used.</param>
    /// <param name="paramName">The name of the parameter being validated. This is automatically populated by the compiler.</param>
    /// <returns>The validated list if it is not null or empty.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="list"/> is null or empty.</exception>
    public static IList<T> IsNullThrow<T>(
    [NotNullWhen(false)] this IList<T>? list,
    string? message = null,
    [CallerArgumentExpression(nameof(list))] string? paramName = null)
    {
        if (list is null || list.Count == 0)
            throw new ArgumentNullException(paramName, message ?? "List cannot be null or empty.");
        return list;
    }

    /// <summary>
    /// Determines whether the specified object is null or, if it is a string, empty.
    /// </summary>
    /// <typeparam name="T">The type of the object to check.</typeparam>
    /// <param name="obj">The object to check for null or empty.</param>
    /// <returns>
    /// <c>true</c> if the object is null or, if it is a string, empty; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this T obj)
    {
        if (obj is string str)
            return string.IsNullOrEmpty(str);

        return obj is null;
    }

    /// <summary>
    /// Determines whether the specified object is not null.
    /// </summary>
    /// <typeparam name="T">The type of the object to check.</typeparam>
    /// <param name="obj">The object to check for not null.</param>
    /// <returns><c>true</c> if the object is not null; otherwise, <c>false</c>.</returns>
    public static bool IsNotNull<T>([NotNullWhen(false)] this T obj)
    {
        if (obj is string str)
            return !string.IsNullOrEmpty(str);

        return obj is not null;
    }
}

#nullable enable
/// <summary>
/// Provides extension methods for <see cref="Dictionary{TKey, TValue}"/> to enhance functionality.
/// </summary>
/// <remarks>This class includes methods that simplify common operations on dictionaries, such as removing entries
/// with null values.</remarks>
public static class DictionaryExtensions
{
    /// <summary>
    /// Removes all entries with <see langword="null"/> values from the specified dictionary.
    /// </summary>
    /// <remarks>This method iterates over the dictionary and removes any key-value pairs where the value is 
    /// <see langword="null"/>. The operation modifies the original dictionary and also returns it  for
    /// convenience.</remarks>
    /// <param name="dict">The dictionary from which to remove entries with <see langword="null"/> values.  This dictionary is modified in
    /// place.</param>
    /// <returns>The modified dictionary with all entries containing <see langword="null"/> values removed.</returns>
    public static Dictionary<string, object?> RemoveNullValues(this Dictionary<string, object?> dict)
    {
        var keys = new List<string>(dict.Keys);
        foreach (var key in keys)
        {
            if (dict[key] == null)
                dict.Remove(key);
        }
        return dict;
    }
}
