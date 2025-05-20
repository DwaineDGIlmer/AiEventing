using Core.Serializers;
using Loggers.Contracts;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace UnitTests
{
    /// <summary>
    /// Provides a base class for unit tests, offering utility methods and configurations to simplify the setup of test
    /// environments.
    /// </summary>
    /// <remarks>This class includes functionality for initializing shared services, creating mock HTTP
    /// clients, and generating test log events. It is designed to streamline common tasks in unit testing scenarios,
    /// such as mocking HTTP responses or serializing log events.</remarks>
    public class UnitTestsBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitTestsBase"/> class.
        /// </summary>
        /// <remarks>Ensures that the <see cref="JsonConvertService"/> is initialized with default
        /// settings if it has not already been initialized. This is necessary to provide consistent JSON serialization
        /// behavior for unit tests.</remarks>
        public UnitTestsBase()
        {
            if (!JsonConvertService.IsInitialized)
            {
                JsonConvertService.Initialize(new JsonSerializerOptions()
                {
                    WriteIndented = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                });
            }
        }

        /// <summary>
        /// Represents a log event with detailed information such as timestamp, message body, trace identifiers, and
        /// exception details.
        /// </summary>
        /// <remarks>This class is commonly used to capture and represent log data in structured logging
        /// systems.  It includes properties for traceability (e.g., <see cref="TraceId"/> and <see cref="SpanId"/>), 
        /// contextual information (e.g., <see cref="CorrelationId"/>), and error details (e.g., <see cref="Exception"/>
        /// and <see cref="StackTrace"/>).</remarks>
        public class TestLogEvent : ILogEvent
        {
            /// <summary>
            /// <inheritdoc cref="ILogEvent.Timestamp"/>/>
            /// </summary>
            public DateTimeOffset Timestamp { get; set; }

            /// <summary>
            /// <inheritdoc cref="ILogEvent.Body"/>/>
            /// </summary>
            public string Body { get; set; } = string.Empty;

            /// <summary>
            /// <inheritdoc cref="ILogEvent.TraceId"/>
            /// </summary>  
            public string TraceId { get; set; } = string.Empty;

            /// <summary>
            /// <inheritdoc cref="ILogEvent.SpanId"/>
            /// </summary>  
            public string SpanId { get; set; } = string.Empty;

            /// <summary>
            /// <inheritdoc cref="ILogEvent.Level"/>
            /// </summary>
            public LogLevel Level { get; set; }

            /// <summary>
            /// <inheritdoc cref="ILogEvent.Source"/>
            /// </summary>
            public string Source { get; set; } = string.Empty;

            /// <summary>
            /// <inheritdoc cref="ILogEvent.CorrelationId"/>
            /// </summary>
            public string? CorrelationId { get; set; }

            /// <summary>
            /// <inheritdoc cref="ILogEvent.Exception"/>
            /// </summary>
            public Exception? Exception { get; set; }

            /// <summary>
            /// <inheritdoc cref="ILogEvent.StackTrace"/>
            /// </summary>
            public System.Diagnostics.StackTrace? StackTrace { get; set; }

            /// <summary>
            /// Serializes the current instance into a JSON string representation.
            /// </summary>
            /// <remarks>This method uses the <see cref="JsonConvertService"/> to perform the
            /// serialization. Ensure that all properties of the instance are serializable to avoid runtime
            /// errors.</remarks>
            /// <returns>A JSON string that represents the current instance.</returns>
            public string Serialize() => JsonConvertService.Instance!.Serialize(this);
        }

        /// <summary>
        /// Gets a factory method for creating instances of <see cref="TestLogEvent"/>.
        /// </summary>
        public static Func<ILogEvent> TestLogEventFactory => () => new TestLogEvent();

        /// <summary>
        /// Creates a mock <see cref="HttpClient"/> instance that returns a predefined response.
        /// </summary>
        /// <remarks>The returned <see cref="HttpClient"/> is configured with a base address of
        /// "http://localhost/". This method is useful for testing components that depend on <see cref="HttpClient"/>
        /// without making actual HTTP requests.</remarks>
        /// <param name="responseContent">The content to include in the mock HTTP response.</param>
        /// <param name="statusCode">The HTTP status code to return in the mock response. Defaults to <see cref="HttpStatusCode.OK"/>.</param>
        /// <returns>A mock <see cref="HttpClient"/> configured to return the specified response content and status code.</returns>
        public HttpClient GetMockHttpClient(string responseContent, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(responseContent),
                })
                .Verifiable();
            return new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost/"),
            };
        }

        /// <summary>
        /// Creates a mock <see cref="HttpClient"/> that throws a specified exception for all HTTP requests.
        /// </summary>
        /// <remarks>The returned <see cref="HttpClient"/> is intended for use in unit tests where
        /// simulating failure scenarios is required. The <see cref="HttpClient.BaseAddress"/> is set to
        /// "http://localhost/" by default.</remarks>
        /// <param name="exceptionToThrow">The <see cref="Exception"/> to be thrown when any HTTP request is made using the returned <see
        /// cref="HttpClient"/>.</param>
        /// <returns>A mock <see cref="HttpClient"/> instance configured to throw the specified exception for all requests.</returns>
        public HttpClient GetMockHttpClient(Exception exceptionToThrow)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(exceptionToThrow)
                .Verifiable();
            return new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost/"),
            };
        }
    }

    /// <summary>
    /// A mock implementation of the <see cref="IPublisher"/> interface for testing purposes.
    /// </summary>
    /// <remarks>This class provides a simple in-memory implementation of a message publisher, storing
    /// messages in a list and optionally writing them to the console. It is intended for use in unit tests or scenarios
    /// where a lightweight, non-production publisher is needed.</remarks>
    public class MockPublisher : IPublisher
    {
        /// <summary>
        /// Gets the collection of messages.
        /// </summary>
        public IList<string> Messages { get; } = new List<string>();

        /// <summary>
        /// Determines whether the collection contains any message that includes the specified substring.
        /// </summary>
        /// <param name="message">The substring to search for within the messages. Cannot be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if at least one message in the collection contains the specified substring;
        /// otherwise, <see langword="false"/>.</returns>
        public bool Contains(string message) => Messages.Any(m => m.Contains(message));

        /// <summary>
        /// <inheritdoc cref="IPublisher.Write(string)"/>
        /// </summary>
        public Task Write(string message)
        {
            Messages.Add(message);
            Console.WriteLine(message);
            return Task.CompletedTask;
        }

        /// <summary>
        /// <inheritdoc cref="IPublisher.WriteLine(string)"/>
        /// </summary>
        public Task WriteLine(string message)
        {
            Messages.Add(message);
            Console.WriteLine(message);
            return Task.CompletedTask;
        }
    }
}
