using Core.Models;
using Microsoft.Extensions.Logging;

namespace Core.UnitTests.Models;

public class ExceptionContextTest
{
    [Fact]
    public void DefaultPropertyValues_AreSetCorrectly()
    {
        var context = new ExceptionContext();

        Assert.Equal(string.Empty, context.Id);
        Assert.Equal(string.Empty, context.Application);
        Assert.Equal(LogLevel.Error, context.SeverityLevel);
        Assert.Equal(string.Empty, context.FaultId);
        Assert.Equal(string.Empty, context.TraceId);
        Assert.Equal(string.Empty, context.SpanId);
        Assert.Null(context.Exception);
        Assert.Equal(string.Empty, context.ExceptionType);
        Assert.Equal(string.Empty, context.StackTrace);
        Assert.Equal(string.Empty, context.Service);
        Assert.Equal(string.Empty, context.Environment);
        Assert.Equal(string.Empty, context.SourceFile);
        Assert.Equal(0, context.LineNumber);
        Assert.Equal(string.Empty, context.Message);
        Assert.Equal(string.Empty, context.Source);
        Assert.Equal(string.Empty, context.Method);
        Assert.Equal(string.Empty, context.TargetSite);
        Assert.NotNull(context.InnerException);
        Assert.Equal(Environment.CurrentManagedThreadId, context.ThreadId);
        Assert.False(string.IsNullOrWhiteSpace(context.ExceptionId));
        Assert.False(string.IsNullOrWhiteSpace(context.ExceptionOccurrenceId));
        Assert.Null(context.CorrelationId);
        Assert.NotNull(context.CustomProperties);
    }

    [Fact]
    public void ExceptionType_TrimsAndExtractsTypeName()
    {
        var context = new ExceptionContext
        {
            ExceptionType = "System.ArgumentException"
        };
        Assert.Equal("ArgumentException", context.ExceptionType);

        context.ExceptionType = "ArgumentException";
        Assert.Equal("ArgumentException", context.ExceptionType);

        context.ExceptionType = "   System.InvalidOperationException   ";
        Assert.Equal("InvalidOperationException", context.ExceptionType);

        context.ExceptionType = "";
        Assert.Equal(string.Empty, context.ExceptionType);

        context.ExceptionType = null!;
        Assert.Equal(string.Empty, context.ExceptionType);
    }

    [Fact]
    public void CustomProperties_CanBeSetAndRetrieved()
    {
        var context = new ExceptionContext();
        context.CustomProperties["key1"] = "value1";
        context.CustomProperties["key2"] = 123;

        Assert.Equal("value1", context.CustomProperties["key1"]);
        Assert.Equal(123, context.CustomProperties["key2"]);
    }
}