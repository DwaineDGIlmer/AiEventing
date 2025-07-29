using Core.Extensions;

namespace Core.UnitTests.Extensions;

public class ExtensionsTest
{
    [Fact]
    public void IsNull_ShouldReturnTrueIfObjectIsNull()
    {
        // Arrange
        object? obj = null;

        // Act
        var result = obj.IsNull();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsNull_ShouldReturnFalseIfObjectIsNotNull()
    {
        // Arrange
        var obj = new object();

        // Act
        var result = obj.IsNull();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsNullOrEmpty_ShouldReturnTrueIfStringIsNull()
    {
        // Arrange
        string? str = null;

        // Act
        var result = str.IsNullOrEmpty();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsNullOrEmpty_ShouldReturnTrueIfStringIsEmpty()
    {
        // Arrange
        string str = "";

        // Act
        var result = str.IsNullOrEmpty();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsNullOrEmpty_ShouldReturnFalseIfStringIsNotEmpty()
    {
        // Arrange
        string str = "Test";

        // Act
        var result = str.IsNullOrEmpty();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsNotNull_ShouldReturnTrueIfObjectIsNotNull()
    {
        // Arrange
        var obj = new object();

        // Act
        var result = obj.IsNotNull();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsNotNull_ShouldReturnFalseIfObjectIsNull()
    {
        // Arrange
        object? obj = null;

        // Act
        var result = obj.IsNotNull();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsNullThrow_WithStringValue_Success()
    {
        string value = "Testing!";
        var results = value.IsNullThrow();
        Assert.Equal(value, results);
    }

    [Fact]
    public void IsNullThrow_WithNullString_ThrowsArgumentNullException()
    {
        string value = null!;
        Assert.Throws<ArgumentNullException>(() => value.IsNullThrow());
    }

    [Fact]
    public void IsNullThrow_WithEmptyString_ThrowsArgumentNullException()
    {
        string value = "";
        Assert.Throws<ArgumentNullException>(() => value.IsNullThrow());
    }

    [Fact]
    public void IsNullThrow_WithNonEmptyString_DoesNotThrow()
    {
        string value = "test";
        value.IsNullThrow(); // Should not throw
    }

    [Fact]
    public void IsNullThrow_WithNonStringNullObject_Doeshrow()
    {
        object value = null!;
        Assert.Throws<ArgumentNullException>(() => value.IsNullThrow()); // Should throw
    }

    [Fact]
    public void IsNullThrow_WithNonStringNonNullObject_DoesNotThrow()
    {
        object value = new();
        value.IsNullThrow(); // Should not throw
    }

    [Fact]
    public void IsNullThrow_WithNullList_ThrowsArgumentNullException()
    {
        IList<string> value = null!;
        Assert.Throws<ArgumentNullException>(() => value.IsNullThrow());
    }

    [Fact]
    public void IsNullThrow_WithNonList_DoesNotThrow()
    {
        IList<string> value = new List<string>() { "test" };
        value.IsNullThrow(); // Should not throw
    }

    class TestClass
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    [Fact]
    public void GenHash_ShouldReturnZeroForNullObject()
    {
        object? obj = null;
        var result = obj.GenHash();
        Assert.Equal(0UL, result);
    }

    [Fact]
    public void GenHash_ShouldReturnNonZeroForNonNullObject()
    {
        var obj = new { Name = "Test", Value = 123 };
        var result = obj.GenHash();
        Assert.NotEqual(0UL, result);
    }

    [Fact]
    public void GenHashString_ShouldReturnEmptyForNullObject()
    {
        object? obj = null;
        var result = obj.GenHashString();
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GenHashString_ShouldReturnNonEmptyForNonNullObject()
    {
        var obj = new { Name = "Test", Value = 123 };
        var result = obj.GenHashString();
        Assert.False(string.IsNullOrEmpty(result));
    }

    [Fact]
    public void ToJson_ShouldSerializeObject()
    {
        var obj = new { Name = "Test", Value = 123 };
        var json = obj.ToJson();
        Assert.Contains("Test", json);
        Assert.Contains("123", json);
    }

    [Fact]
    public void ToJsonTry_ShouldReturnEmptyOnSerializationError()
    {
        var obj = new object();
        var json = obj.ToJsonTry();
        Assert.True(json.Length > 0); // Should serialize, as object is serializable
    }

    [Fact]
    public void ToJsonTry_With_Options_ShouldReturnEmptyOnSerializationError()
    {
        var obj = new object();
        var json = obj.ToJsonTry(new System.Text.Json.JsonSerializerOptions());
        Assert.True(json.Length > 0); // Should serialize, as object is serializable
    }

    [Fact]
    public void ToObject_ShouldDeserializeJson()
    {
        var json = "{\"Name\":\"Test\",\"Value\":123}";
        var obj = json.ToObject<TestClass>();
        Assert.Equal("Test", obj.Name);
        Assert.Equal(123, obj.Value);
    }

    [Fact]
    public void ToObjectTry_ShouldReturnDefaultOnError()
    {
        var json = "invalid json";
        var obj = json.ToObjectTry<TestClass>();
        Assert.Null(obj);
    }

    [Fact]
    public void RemoveNullValues_RemovesEntriesWithNullValues()
    {
        var dict = new Dictionary<string, object?>()
        {
            { "a", 1 },
            { "b", null },
            { "c", "test" }
        };
        var result = dict.RemoveNullValues();
        Assert.False(result.ContainsKey("b"));
        Assert.True(result.ContainsKey("a"));
        Assert.True(result.ContainsKey("c"));
    }
}
