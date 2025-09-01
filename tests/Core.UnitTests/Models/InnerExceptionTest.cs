using Core.Models;

namespace Core.UnitTests.Models;

public sealed class InnerExceptionTest
{
    [Fact]
    public void Constructor_ValidException_SetsPropertiesCorrectly()
    {
        // Arrange
        var exception = new InvalidOperationException("Test message");

        // Act
        var innerException = new SerializableException(exception);

        // Assert
        Assert.Equal(typeof(InvalidOperationException).FullName, innerException.ExceptionType);
        Assert.Equal("Test message", innerException.ExceptionMessage);
        Assert.NotNull(innerException.ExceptionStackTrace);
    }

    [Fact]
    public void Constructor_ExceptionWithNullProperties_UsesDefaultValues()
    {
        // Arrange
        var exception = new Exception();

        // Act
        var innerException = new SerializableException(exception);

        // Assert
        Assert.Equal(typeof(Exception).FullName, innerException.ExceptionType);
        Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message == null ? innerException.ExceptionMessage : innerException.ExceptionMessage);
        Assert.NotNull(innerException.ExceptionStackTrace);
    }
}