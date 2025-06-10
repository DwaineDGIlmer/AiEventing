using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Core.Configuration;
using Core.Contracts;
using Loggers.Application;
using Loggers.Models;
using Microsoft.Extensions.Logging;

namespace LoggerBenchMarkTests;

public class OtelLogEventsBenchmarks
{
    private static Func<ILogEvent> TestLogEventFactory => () => new OtelLogEvents();
    private readonly ApplicationLogger _logger = new("BenchmarkLogger", new AiEventSettings()
    {
        MinLogLevel = LogLevel.Information,
    }, TestLogEventFactory);
    private readonly OtelLogEvents _logEvent = new()
    {
        Body = "Benchmark test message",
        Level = LogLevel.Warning,
        Source = "Benchmark",
        TraceId = Guid.NewGuid().ToString("N"),
        SpanId = Guid.NewGuid().ToString("N"),
        CorrelationId = Guid.NewGuid().ToString("N"),
        Exception = new(new InvalidOperationException("Benchmark exception"))
    };

    [Benchmark]
    public string Serialize_OtelLogEvent()
    {
        return _logEvent.Serialize();
    }

    [Benchmark]
    public void Logger_Performance()
    {
        _logger.Log(LogLevel.Information, new EventId(1, "TestEvent"), "state", null, (s, e) => $"Message: {s}");
    }
}

public class Program
{
    public static void Main()
    {
        BenchmarkRunner.Run<OtelLogEventsBenchmarks>();
    }
}