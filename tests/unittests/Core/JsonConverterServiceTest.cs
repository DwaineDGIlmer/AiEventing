using Core.Serializers;

namespace UnitTests.Core;

public class JsonConvertServiceTest
{
    public JsonConvertServiceTest()
    {
        // Reset singleton before each test to ensure isolation
        ResetSingleton();
    }

    [Fact]
    public void DefaultConstructor_InitializesInstanceAndOptions()
    {
        // Arrange
        ResetSingleton();

        // Act
        var service = new JsonConvertService();

        // Assert
        Assert.NotNull(JsonConvertService.Instance);
        Assert.True(JsonConvertService.IsInitialized);
        Assert.NotNull(service);
        Assert.NotNull(service.Serialize(new { Test = 1 }));
    }

    [Fact]
    public void Serialize_And_Deserialize_WorksCorrectly()
    {
        // Arrange
        ResetSingleton();
        var service = new JsonConvertService();
        var obj = new TestClass { Id = 42, Name = "Test" };

        // Act
        var json = service.Serialize(obj);
        var result = service.Deserialize<TestClass>(json);

        // Assert
        Assert.Equal(obj.Id, result.Id);
        Assert.Equal(obj.Name, result.Name);
    }

    [Fact]
    public void EnsureInitialized_ThrowsIfNotInitialized()
    {
        // Arrange
        ResetSingleton();

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => JsonConvertService.EnsureInitialized());
        Assert.Contains("Component has not been initialized", ex.Message);
    }

    [Fact]
    public void Instance_Setter_ThrowsIfNull()
    {
        // Arrange
        ResetSingleton();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => JsonConvertService.Instance = null!);
    }

    private void ResetSingleton()
    {
        typeof(JsonConvertService)
            .GetField("_instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
            ?.SetValue(null, null);
        typeof(JsonConvertService)
            .GetProperty("IsInitialized", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
            ?.SetValue(null, false);
    }

    private class TestClass
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}