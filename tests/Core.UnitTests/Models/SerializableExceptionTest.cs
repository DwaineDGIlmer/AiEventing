using Core.Models;

namespace Core.UnitTests.Models;

public sealed class SerializableExceptionTest
{
    [Fact]
    public void DefaultConstructor_InitializesPropertiesToEmpty()
    {
        var ex = new SerializableException();

        Assert.Equal(string.Empty, ex.ExceptionType);
        Assert.Equal(string.Empty, ex.ExceptionMessage);
        Assert.Equal(string.Empty, ex.ExceptionStackTrace);
        Assert.NotNull(ex.InnerExceptions);
        Assert.Empty(ex.InnerExceptions);
    }

    [Fact]
    public void Constructor_WithException_SetsPropertiesCorrectly()
    {
        var inner = new InvalidOperationException("Inner error");
        var exception = new Exception("Test message", inner);

        var serializableEx = new SerializableException(exception);

        Assert.Equal(exception.GetType().FullName, serializableEx.ExceptionType);
        Assert.Equal(exception.Message, serializableEx.ExceptionMessage);
        Assert.Equal(exception.StackTrace ?? string.Empty, serializableEx.ExceptionStackTrace);
        Assert.NotNull(serializableEx.InnerExceptions);
    }

    [Fact]
    public void Constructor_WithNullException_PropertiesAreEmpty()
    {
        var serializableEx = new SerializableException(null!);

        Assert.Equal(string.Empty, serializableEx.ExceptionType);
        Assert.Equal(string.Empty, serializableEx.ExceptionMessage);
        Assert.Equal(string.Empty, serializableEx.ExceptionStackTrace);
        Assert.NotNull(serializableEx.InnerExceptions);
    }

    [Fact]
    public void ToString_ReturnsFormattedString()
    {
        var inner = new Exception("Inner");
        var ex = new Exception("Outer", inner);
        var serializableEx = new SerializableException(ex);

        var result = serializableEx.ToString();

        Assert.Contains("Exception Type:", result);
        Assert.Contains("Message:", result);
        Assert.Contains("Stack Trace:", result);
        if (serializableEx.InnerExceptions.Count > 0)
        {
            Assert.Contains("Inner Exceptions:", result);
            Assert.Contains("Inner", result);
        }
    }

    [Fact]
    public void InnerExceptions_Setter_AllowsManualAssignment()
    {
        var ex = new SerializableException();
        ex.InnerExceptions = new List<SerializableException>
        {
            new SerializableException { ExceptionType = "TestType", ExceptionMessage = "TestMsg" }
        };

        Assert.Single(ex.InnerExceptions);
        Assert.Equal("TestType", ex.InnerExceptions[0].ExceptionType);
    }
}