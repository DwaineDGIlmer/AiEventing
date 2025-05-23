using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

[assembly: InternalsVisibleTo("IntegrationTests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c599f69c3dd3ec398aa236557324d13db0f01fe1619e95bd66ab4fbd53143b2e57470a9c156080f2e3b088da0a7d40ce549ed7d803bc7cfc904077dce8ea5262c4afc77594841fb916db84485db81dfa6ba6cba1d449c0cb8c6aafd42245221dc310fae03f9b18c258c7939cd293f01a9b1ab9c433a53b278022f02a46958797")]
[assembly: InternalsVisibleTo("Loggers, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c599f69c3dd3ec398aa236557324d13db0f01fe1619e95bd66ab4fbd53143b2e57470a9c156080f2e3b088da0a7d40ce549ed7d803bc7cfc904077dce8ea5262c4afc77594841fb916db84485db81dfa6ba6cba1d449c0cb8c6aafd42245221dc310fae03f9b18c258c7939cd293f01a9b1ab9c433a53b278022f02a46958797")]
namespace Core.Serializers;

/// <summary>
/// Provides JSON serialization and deserialization functionality with configurable options.
/// Implements a singleton pattern for global access, but supports DI and testability.
/// </summary>
/// <remarks>
/// Constructor for DI or manual instantiation.
/// </remarks>
internal class JsonConvertService(JsonSerializerOptions options)
{
    private static JsonConvertService? _instance;
    private static bool _isInitialized = false;

    /// <summary>
    /// Gets the singleton instance of the <see cref="JsonConvertService"/>.
    /// </summary>
    public static JsonConvertService Instance
    {
        get
        {
            if (_instance == null)
            {
                Initialize();
            }
            return _instance!;
        }
        internal set
        {
            _instance = value ?? throw new ArgumentNullException(nameof(value), "Instance cannot be null.");
            _isInitialized = true;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the component has been initialized.
    /// </summary>
    public static bool IsInitialized => _isInitialized && _instance != null;

    /// <summary>
    /// Gets the <see cref="JsonSerializerOptions"/> used for serialization and deserialization.
    /// </summary>
    internal JsonSerializerOptions Options { get; private set; } = options ?? throw new ArgumentNullException(nameof(options));

    /// <summary>
    /// Default constructor for DI. Uses default options.
    /// </summary>
    public JsonConvertService() : this(new JsonSerializerOptions
    {
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    })
    {
    }

    /// <summary>
    /// Initializes the singleton instance for static usage.
    /// </summary>
    public static void Initialize(JsonSerializerOptions? options = null)
    {
        if (_instance != null)
            return;

        _instance = new JsonConvertService(options ?? new JsonSerializerOptions
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });
        _isInitialized = true;
    }

    /// <summary>
    /// For testing only: resets the singleton instance.
    /// </summary>
    public static void ResetForTesting()
    {
        _instance = null;
        _isInitialized = false;
    }

    /// <summary>
    /// Serializes the specified object to a JSON string.
    /// </summary>
    /// <remarks>This method uses the <see cref="JsonSerializer"/> for serialization.  If no
    /// options are provided, the default options configured for the serializer will be applied.</remarks>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="obj">The object to serialize. Cannot be <see langword="null"/>.</param>
    /// <param name="options">Optional <see cref="JsonSerializerOptions"/> to customize the serialization process.  If <see langword="null"/>,
    /// default options will be used.</param>
    /// <returns>A JSON string representation of the specified object.</returns>
    public string Serialize<T>(T obj, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Serialize(obj, options ?? Options);
    }

    /// <summary>
    /// Deserializes the specified JSON string into an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>This method uses the configured <see cref="JsonSerializerOptions"/> to perform the
    /// deserialization. Ensure that the JSON string is valid and matches the structure of the target type <typeparamref
    /// name="T"/>.</remarks>
    /// <typeparam name="T">The type of the object to deserialize the JSON string into.</typeparam>
    /// <param name="json">The JSON string to deserialize. Cannot be <see langword="null"/> or empty.</param>
    /// <returns>An object of type <typeparamref name="T"/> that represents the deserialized JSON string.</returns>
    /// <exception cref="JsonException">Thrown if the JSON string cannot be deserialized into an object of type <typeparamref name="T"/>.</exception>
    public T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, Options) ?? throw new JsonException("Deserialization failed.");
    }
}