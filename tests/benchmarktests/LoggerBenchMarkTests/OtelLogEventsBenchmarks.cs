using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Core.Application;
using Core.Configuration;
using Loggers.Contracts;
using Loggers.Models;
using Microsoft.Extensions.Logging;

public class OtelLogEventsBenchmarks
{
    private static Func<ILogEvent> TestLogEventFactory => () => new OtelLogEvents();
    private ApplicationLogger _logger = new ApplicationLogger("BenchmarkLogger", new AiEventSettings()
    {
        MinLogLevel = LogLevel.Information,
    }, TestLogEventFactory);
    private OtelLogEvents _logEvent = new OtelLogEvents
    {
        Body = "Benchmark test message",
        Level = LogLevel.Warning,
        Source = "Benchmark",
        TraceId = Guid.NewGuid().ToString("N"),
        SpanId = Guid.NewGuid().ToString("N"),
        CorrelationId = Guid.NewGuid().ToString("N"),
        Exception = new InvalidOperationException("Benchmark exception")
    };

    [GlobalSetup]
    public void Setup() { }

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
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<OtelLogEventsBenchmarks>();
    }
}