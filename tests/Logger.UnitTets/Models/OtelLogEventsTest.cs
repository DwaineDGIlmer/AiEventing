using Loggers.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json.Nodes;

namespace Logger.UnitTests.Models;

public class OtelLogEventsTest : UnitTestsBase
{
    [Fact]
    public void Serialize_DefaultValues_ProducesExpectedJson()
    {
        // Arrange: Create a log event with default values
        var logEvent = new OtelLogEvents();

        // Act: Serialize and parse the result
        var json = logEvent.Serialize();
        var doc = JsonNode.Parse(json)!;

        // Assert: Top-level fields
        Assert.True(doc["timestamp"]!.GetValue<long>() > 0);
        Assert.Equal("ERROR", doc["severity_text"]!.GetValue<string>());
        Assert.Equal((int)LogLevel.Error, doc["severity_number"]!.GetValue<int>());
        Assert.Equal(string.Empty, doc["body"]!.GetValue<string>());
        Assert.Null(doc["trace_id"]);
        Assert.Null(doc["span_id"]);

        // Assert: Attributes object contains only "source" with empty string
        var attributes = doc["attributes"]!.AsObject();
        Assert.Empty(attributes);
        Assert.False(attributes.ContainsKey("source"));

        // Assert: No other keys are present
        var unexpectedKeys = new[] { "correlation_id", "exception.type", "exception.message", "exception.stacktrace" };
        foreach (var key in unexpectedKeys)
            Assert.False(attributes.ContainsKey(key), $"Attribute '{key}' should not be present.");
    }

    [Fact]
    public void Serialize_WithTraceAndSpanIds_IncludesIds()
    {
        var logEvent = new OtelLogEvents
        {
            TraceId = "abc123",
            SpanId = "def456"
        };

        var json = logEvent.Serialize();
        var doc = JsonNode.Parse(json)!;

        Assert.Equal("abc123", doc["trace_id"]!.GetValue<string>());
        Assert.Equal("def456", doc["span_id"]!.GetValue<string>());
    }

    [Fact]
    public void Serialize_WithCorrelationId_IncludesCorrelationId()
    {
        var logEvent = new OtelLogEvents
        {
            CorrelationId = "corr-789"
        };

        var json = logEvent.Serialize();
        var doc = JsonNode.Parse(json)!;
        var attributes = doc["attributes"]!.AsObject();

        Assert.Equal("corr-789", attributes["correlation_id"]!.GetValue<string>());
    }

    [Fact]
    public void Serialize_WithException_IncludesExceptionFields()
    {
        Exception ex;
        try
        {
            throw new InvalidOperationException("fail!", new Exception("Inner fail!!"));
        }
        catch (Exception caught)
        {
            ex = caught;
        }
        var logEvent = new OtelLogEvents
        {
            Exception = new(ex)
        };

        var json = logEvent.Serialize();
        var doc = JsonNode.Parse(json)!;
        var attributes = doc["attributes"]!.AsObject();

        Assert.Equal(typeof(InvalidOperationException).FullName, attributes["exception.type"]!.GetValue<string>());
        Assert.Equal("fail!", attributes["exception.message"]!.GetValue<string>());
        Assert.NotNull(attributes["exception.stacktrace"]);
    }

    [Fact]
    public void Serialize_WithStackTrace_OverridesExceptionStackTrace()
    {
        var ex = new Exception("msg");
        var logEvent = new OtelLogEvents
        {
            Exception = new(ex)
        };

        var json = logEvent.Serialize();
        var doc = JsonNode.Parse(json)!;
        var attributes = doc["attributes"]!.AsObject();

        Assert.False(attributes.ContainsKey("exception.stacktrace"));
    }

    [Fact]
    public void Serialize_OmitsNullAndEmptyFields()
    {
        var logEvent = new OtelLogEvents
        {
            TraceId = "",
            SpanId = "",
            CorrelationId = null,
            Exception = null!
        };

        var json = logEvent.Serialize();
        var doc = JsonNode.Parse(json)!;

        Assert.Null(doc["trace_id"]);
        Assert.Null(doc["span_id"]);
        var attributes = doc["attributes"]!.AsObject();
        Assert.False(attributes.ContainsKey("correlation_id"));
        Assert.False(attributes.ContainsKey("exception.type"));
        Assert.False(attributes.ContainsKey("exception.message"));
        Assert.False(attributes.ContainsKey("exception.stacktrace"));
    }
}