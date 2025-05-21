using Loggers.Models;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json.Nodes;

namespace UnitTests.Loggers;

public class OtelLogEventsTest : UnitTestsBase
{
    [Fact]
    public void Serialize_DefaultValues_ProducesExpectedJson()
    {
        var logEvent = new OtelLogEvents();

        var json = logEvent.Serialize();
        var doc = JsonNode.Parse(json)!;

        Assert.True(doc["timestamp"]!.GetValue<long>() > 0);
        Assert.Equal("INFORMATION", doc["severity_text"]!.GetValue<string>());
        Assert.Equal((int)LogLevel.Information, doc["severity_number"]!.GetValue<int>());
        Assert.Equal(string.Empty, doc["body"]!.GetValue<string>());
        Assert.Null(doc["trace_id"]);
        Assert.Null(doc["span_id"]);

        var attributes = doc["attributes"]!.AsObject();
        Assert.Equal(string.Empty, attributes["source"]!.GetValue<string>());
        Assert.False(attributes.ContainsKey("correlation_id"));
        Assert.False(attributes.ContainsKey("exception.type"));
        Assert.False(attributes.ContainsKey("exception.message"));
        Assert.False(attributes.ContainsKey("exception.stacktrace"));
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
            Exception = ex
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
        var stackTrace = new StackTrace();
        var logEvent = new OtelLogEvents
        {
            Exception = ex,
            StackTrace = stackTrace
        };

        var json = logEvent.Serialize();
        var doc = JsonNode.Parse(json)!;
        var attributes = doc["attributes"]!.AsObject();

        Assert.Contains(nameof(OtelLogEventsTest), attributes["exception.stacktrace"]!.GetValue<string>());
    }

    [Fact]
    public void Serialize_OmitsNullAndEmptyFields()
    {
        var logEvent = new OtelLogEvents
        {
            TraceId = "",
            SpanId = "",
            CorrelationId = null,
            Exception = null,
            StackTrace = null
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