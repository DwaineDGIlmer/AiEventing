using Core.Extensions;
using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

[assembly: InternalsVisibleTo("IntegrationTests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c599f69c3dd3ec398aa236557324d13db0f01fe1619e95bd66ab4fbd53143b2e57470a9c156080f2e3b088da0a7d40ce549ed7d803bc7cfc904077dce8ea5262c4afc77594841fb916db84485db81dfa6ba6cba1d449c0cb8c6aafd42245221dc310fae03f9b18c258c7939cd293f01a9b1ab9c433a53b278022f02a46958797")]
[assembly: InternalsVisibleTo("Loggers, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c599f69c3dd3ec398aa236557324d13db0f01fe1619e95bd66ab4fbd53143b2e57470a9c156080f2e3b088da0a7d40ce549ed7d803bc7cfc904077dce8ea5262c4afc77594841fb916db84485db81dfa6ba6cba1d449c0cb8c6aafd42245221dc310fae03f9b18c258c7939cd293f01a9b1ab9c433a53b278022f02a46958797")]
namespace Core.Serializers;

/// <summary>
/// Provides JSON serialization and deserialization functionality with configurable options.
/// Implements a singleton pattern for global access.
/// </summary>
internal class JsonConvertService
{
    /// <summary>
    /// Gets the singleton instance of the <see cref="JsonConvertService"/>.
    /// </summary>
    public static JsonConvertService? Instance
    {
        get => _instance;
        internal set
        {
            _instance ??= value ?? throw new ArgumentNullException(nameof(value), "Instance cannot be null.");
            _isInitialized = true;
        }
    }
    private static JsonConvertService? _instance;

    /// <summary>
    /// Gets or sets a value indicating whether the component has been initialized.
    /// </summary>
    public static bool IsInitialized
    {
        get
        {
            return _isInitialized && _instance.IsNotNull();
        }
        internal set => _isInitialized = value;
    }
    private static bool _isInitialized = false;

    /// <summary>
    /// Gets the <see cref="JsonSerializerOptions"/> used for serialization and deserialization.
    /// </summary>
    internal JsonSerializerOptions Options { get; private set; } = new();

    /// <summary>
    /// Used for tsting only.
    /// </summary>
    internal JsonConvertService()
    {
        if (Instance.IsNotNull())
            return;

        Options = new JsonSerializerOptions
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        Instance = this;
        IsInitialized = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonConvertService"/> class with the specified options.
    /// If no options are provided, default options are used.
    /// Updates the singleton <see cref="Instance"/> to this instance.
    /// </summary>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use, or <c>null</c> to use defaults.</param>
    public JsonConvertService(JsonSerializerOptions options)
    {
        Options = options.IsNullThrow();
        IsInitialized = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonConvertService"/> class with the specified options.
    /// If no options are provided, default options are used.
    /// Updates the singleton <see cref="Instance"/> to this instance.
    /// </summary>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use, or <c>null</c> to use defaults.</param>
    public static void Initialize(JsonSerializerOptions options)
    {
        options.IsNullThrow();
        if (Instance != null)
            throw new InvalidOperationException("JsonConvertService is already initialized.");
        Instance = new JsonConvertService(options);
        IsInitialized = true;
    }

    /// <summary>
    /// Serializes the specified object to a JSON string.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="obj">The object to serialize. Cannot be <see langword="null"/>.</param>
    /// <param name="options">Optional <see cref="JsonSerializerOptions"/> to customize the serialization process. If <see langword="null"/>,
    /// default options are used.</param>
    /// <returns>A JSON string representation of the specified object.</returns>
#nullable enable
    public string Serialize<T>(T obj, JsonSerializerOptions? options = null)
    {
        EnsureInitialized();
        return JsonSerializer.Serialize(obj, options ?? Options);
    }
#nullable restore

    /// <summary>
    /// Deserializes the specified JSON string to an object of type <typeparamref name="T"/> using the configured options.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>The deserialized object of type <typeparamref name="T"/>.</returns>
    /// <exception cref="JsonException">Thrown if deserialization fails.</exception>
    public T Deserialize<T>(string json)
    {
        EnsureInitialized();
        return JsonSerializer.Deserialize<T>(json, Options) ?? throw new JsonException("Deserialization failed.");
    }

    /// <summary>
    /// Checks if the component is initialized; throws InvalidOperationException if not.
    /// </summary>
    internal static void EnsureInitialized()
    {
        if (!IsInitialized)
            throw new InvalidOperationException("Component has not been initialized.");
    }
}