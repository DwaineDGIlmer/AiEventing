using Core.Serializers;
using System.Text.Json;

namespace UnitTests.Core;

public class JsonConvertServiceTest
{
    public class TestObj
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    [Fact]
    public void Initialize_SetsInstanceAndIsInitialized()
    {
        JsonConvertService.ResetForTesting();
        JsonConvertService.Initialize();
        Assert.True(JsonConvertService.IsInitialized);
        Assert.NotNull(JsonConvertService.Instance);
    }

    [Fact]
    public void Serialize_And_Deserialize_WorksWithStaticInstance()
    {
        JsonConvertService.ResetForTesting();
        JsonConvertService.Initialize();
        var obj = new TestObj { Name = "foo", Value = 42 };
        var json = JsonConvertService.Instance.Serialize(obj);
        var result = JsonConvertService.Instance.Deserialize<TestObj>(json);
        Assert.Equal(obj.Name, result.Name);
        Assert.Equal(obj.Value, result.Value);
    }

    [Fact]
    public void Serialize_And_Deserialize_WorksWithDIInstance()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var service = new JsonConvertService(options);
        var obj = new TestObj { Name = "bar", Value = 99 };
        var json = service.Serialize(obj);
        var result = service.Deserialize<TestObj>(json);
        Assert.Equal(obj.Name, result.Name);
        Assert.Equal(obj.Value, result.Value);
    }

    [Fact]
    public void Serialize_UsesProvidedOptions()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var service = new JsonConvertService(options);
        var obj = new TestObj { Name = "baz", Value = 1 };
        var json = service.Serialize(obj, new JsonSerializerOptions { WriteIndented = false });
        Assert.DoesNotContain(Environment.NewLine, json); // Not indented
    }

    [Fact]
    public void Deserialize_ThrowsOnInvalidJson()
    {
        JsonConvertService.ResetForTesting();
        JsonConvertService.Initialize();
        Assert.Throws<JsonException>(() => JsonConvertService.Instance.Deserialize<TestObj>("not a json"));
    }

    [Fact]
    public void Constructor_ThrowsOnNullOptions()
    {
        Assert.Throws<ArgumentNullException>(() => new JsonConvertService(null!));
    }
}