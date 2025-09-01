using Core.Models;

namespace Core.UnitTests.Models;

public sealed class ErrorsTest
{
    [Fact]
    public void Error_DefaultValues_AreSetCorrectly()
    {
        // Arrange
        var error = new Error();

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(error.TimeStamp));
        Assert.Equal(string.Empty, error.ErrorCode);
        Assert.Equal(string.Empty, error.ErrorMessage);
        Assert.NotNull(error.ErrorDetails);
        Assert.Empty(error.ErrorDetails);

        // Check timestamp format (ISO 8601)
        DateTime parsed;
        Assert.True(DateTime.TryParse(error.TimeStamp, null, System.Globalization.DateTimeStyles.RoundtripKind, out parsed));
    }

    [Fact]
    public void Error_SetProperties_ValuesAreUpdated()
    {
        // Arrange
        var error = new Error
        {
            ErrorCode = "404",
            ErrorMessage = "Not Found",
            ErrorDetails = new List<string> { "Resource missing", "Check URL" }
        };

        // Assert
        Assert.Equal("404", error.ErrorCode);
        Assert.Equal("Not Found", error.ErrorMessage);
        Assert.Equal(2, error.ErrorDetails.Count);
        Assert.Contains("Resource missing", error.ErrorDetails);
        Assert.Contains("Check URL", error.ErrorDetails);
    }

    [Fact]
    public void Error_ErrorDetails_CanBeModified()
    {
        // Arrange
        var error = new Error();

        // Act
        error.ErrorDetails.Add("First detail");
        error.ErrorDetails.Add("Second detail");

        // Assert
        Assert.Equal(2, error.ErrorDetails.Count);
        Assert.Contains("First detail", error.ErrorDetails);
        Assert.Contains("Second detail", error.ErrorDetails);
    }
}