using Domain.Fault;

namespace Domain.UnitTests.Fault;

public sealed class ExceptionsTest
{
    [Fact]
    public void DefaultConstructor_InitializesPropertiesToEmptyStrings()
    {
        var ex = new Exceptions();

        Assert.Equal(string.Empty, ex.ExceptionType);
        Assert.Equal(string.Empty, ex.ExceptionMessage);
        Assert.Equal(string.Empty, ex.ExceptionStackTrace);
    }

    [Fact]
    public void Properties_SetAndGetValues_Correctly()
    {
        var ex = new Exceptions
        {
            ExceptionType = "System.ArgumentException",
            ExceptionMessage = "Invalid argument",
            ExceptionStackTrace = "at Namespace.Class.Method()"
        };

        Assert.Equal("System.ArgumentException", ex.ExceptionType);
        Assert.Equal("Invalid argument", ex.ExceptionMessage);
        Assert.Equal("at Namespace.Class.Method()", ex.ExceptionStackTrace);
    }
}