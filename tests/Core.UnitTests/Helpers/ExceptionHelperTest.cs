using Core.Helpers;

namespace Core.UnitTests.Helpers;

sealed public class ExceptionHelperTest
{
    [Fact]
    public void GetExceptionHash_SameException_ReturnsSameHash()
    {
        // Arrange
        var ex1 = new InvalidOperationException("Test message");
        var ex2 = new InvalidOperationException("Test message");
        ex1.StackTrace?.ToString(); // Ensure stack trace is generated

        // Act
        var hash1 = ExceptionHelper.GetExceptionHash(ex1);
        var hash2 = ExceptionHelper.GetExceptionHash(ex2);

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void GetExceptionHash_DifferentMessages_ReturnsDifferentHashes()
    {
        // Arrange
        var ex1 = new InvalidOperationException("Message 1");
        var ex2 = new InvalidOperationException("Message 2");

        // Act
        var hash1 = ExceptionHelper.GetExceptionHash(ex1);
        var hash2 = ExceptionHelper.GetExceptionHash(ex2);

        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void GetExceptionHash_DifferentTypes_ReturnsDifferentHashes()
    {
        // Arrange
        var ex1 = new InvalidOperationException("Test");
        var ex2 = new ArgumentException("Test");

        // Act
        var hash1 = ExceptionHelper.GetExceptionHash(ex1);
        var hash2 = ExceptionHelper.GetExceptionHash(ex2);

        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void GetExceptionHash_NullStackTrace_StillReturnsHash()
    {
        // Arrange
        var ex = new Exception("No stack trace");

        // Act
        var hash = ExceptionHelper.GetExceptionHash(ex);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(hash));
    }
}