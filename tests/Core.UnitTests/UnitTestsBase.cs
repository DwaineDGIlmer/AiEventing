#nullable disable
using Core.Contracts;
using Core.Models;
using Core.Serializers;
using Loggers.Contracts;
using Loggers.Publishers;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Diagnostics.Tracing;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.UnitTests
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
        sealed public class TestLogEvent : ILogEvent
        {
            /// <summary>
            /// <inheritdoc cref="ILogEvent.Timestamp"/>/>
            /// </summary>
            public DateTimeOffset Timestamp { get; set; }

            /// <summary>
            /// Gets or sets the unique identifier for the application.
            /// </summary>
            public string ApplicationId { get; set; }

            /// <summary>
            /// Gets or sets the unique identifier for the component.
            /// </summary>
            public string ComponentId { get; set; }

            /// <summary>
            /// Gets or sets the name of the environment in which the application is running.
            /// </summary>
            public string Environment { get; set; }

            /// <summary>
            /// Gets or sets the version of the application or component.
            /// </summary>
            public string Version { get; set; }

            /// <summary>
            /// Gets or sets the unique identifier for the deployment.
            /// </summary>
            public string DeploymentId { get; set; }

            /// <summary>
            /// Gets or sets a collection of key-value pairs representing metadata tags.
            /// </summary>
            /// <remarks>Use this property to store and retrieve metadata associated with an object.  Keys should be
            /// unique within the dictionary to avoid overwriting values.</remarks>
            public IDictionary<string, string> Tags { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

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
            public string CorrelationId { get; set; } = string.Empty;

            /// <summary>
            /// <inheritdoc cref="ILogEvent.Exception"/>
            /// </summary>
            public SerializableException Exception { get; set; }

            /// <summary>
            /// <inheritdoc cref="ILogEvent.StackTrace"/>
            /// </summary>
            public System.Diagnostics.StackTrace StackTrace { get; set; }

            /// <summary>
            /// Gets or sets the unique identifier for the entity.
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// Gets or sets the line number associated with the current operation or context.
            /// </summary>
            public int LineNumber { get; set; }

            /// <summary>
            /// Gets or sets the collection of inner exceptions associated with the current exception.
            /// </summary>
            public IList<SerializableException> InnerExceptions { get; set; } = [];

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
        public static HttpClient GetMockHttpClient(string responseContent, HttpStatusCode statusCode = HttpStatusCode.OK)
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
        public static HttpClient GetMockHttpClient(Exception exceptionToThrow)
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

        /// <summary>
        /// Creates a mock <see cref="HttpClient"/> instance configured to return the specified response message.
        /// </summary>
        /// <remarks>This method is useful for testing scenarios where a controlled response from an <see
        /// cref="HttpClient"/> is required. The returned <see cref="HttpClient"/> uses a mocked <see
        /// cref="HttpMessageHandler"/> to intercept requests and provide the specified response.</remarks>
        /// <param name="responseMessage">The <see cref="HttpResponseMessage"/> to be returned by the mock <see cref="HttpClient"/> for any request.</param>
        /// <returns>A mock <see cref="HttpClient"/> instance that returns the specified <paramref name="responseMessage"/> for
        /// all requests.</returns>
        public static HttpClient CreateMockHttpClient(HttpResponseMessage responseMessage, string baseAddress = null)
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            var client = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress ?? "http://example.com")
            };
            return client;
        }

        /// <summary>
        /// Creates a mock implementation of <see cref="IHttpClientFactory"/> that returns the specified <see
        /// cref="HttpClient"/>  for the given client name.
        /// </summary>
        /// <remarks>This method is useful for testing scenarios where a specific <see cref="HttpClient"/>
        /// instance needs to be injected  into code that depends on <see cref="IHttpClientFactory"/>.</remarks>
        /// <param name="httpClient">The <see cref="HttpClient"/> instance to be returned by the mock factory.</param>
        /// <param name="clientName">The name of the client for which the <see cref="HttpClient"/> will be returned.</param>
        /// <returns>A mock implementation of <see cref="IHttpClientFactory"/> that returns the specified <see
        /// cref="HttpClient"/>  when <see cref="IHttpClientFactory.CreateClient(string)"/> is called with the given
        /// client name.</returns>
        public static IHttpClientFactory CreateMockHttpClientFactory(HttpClient httpClient, string clientName)
        {
            var mockFactory = new Mock<IHttpClientFactory>();
            mockFactory.Setup(f => f.CreateClient(clientName)).Returns(httpClient);
            return mockFactory.Object;
        }


    }

    /// <summary>
    /// A mock implementation of the <see cref="IPublisher"/> interface for testing purposes.
    /// </summary>
    /// <remarks>This class provides a simple in-memory implementation of a message publisher, storing
    /// messages in a list and optionally writing them to the console. It is intended for use in unit tests or scenarios
    /// where a lightweight, non-production publisher is needed.</remarks>
    sealed public class MockPublisher : IPublisher
    {
        /// <summary>
        /// Gets the collection of messages.
        /// </summary>
        public IList<string> Messages { get; } = [];

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

        /// <summary>
        /// A mock implementation of <see cref="EventListener"/> that allows capturing and handling events written by an
        /// <see cref="EventSource"/>.
        /// </summary>
        /// <remarks>This class is designed for testing and debugging purposes, enabling developers to
        /// intercept and process events written by an <see cref="EventSource"/>. The provided callback is invoked
        /// whenever an event is written.</remarks>
        /// <remarks>
        /// Initializes a new instance of the <see cref="MockEventListener"/> class with a specified callback
        /// </remarks>
        /// <param name="onEventWritten"></param>
        sealed public class MockEventListener(Action<EventWrittenEventArgs> onEventWritten) : EventListener
        {
            /// <summary>
            /// Represents a callback action that is invoked when an event is written.
            /// </summary>
            /// <remarks>This delegate is used to handle event data encapsulated in an <see
            /// cref="EventWrittenEventArgs"/> instance. It is typically invoked when an event is logged or written by
            /// an event source.</remarks>
            private readonly Action<EventWrittenEventArgs> _onEventWritten = onEventWritten;

            /// <summary>
            /// Handles the event when an event is written to the event source.
            /// </summary>
            /// <remarks>This method is invoked automatically when an event is written to the event
            /// source.  Override this method to customize how event data is processed or handled.</remarks>
            /// <param name="eventData">The data associated with the event that was written. This includes details such as the event ID,
            /// payload, and message.</param>
            protected override void OnEventWritten(EventWrittenEventArgs eventData)
            {
                _onEventWritten(eventData);
            }
        }
    }

    /// <summary>
    /// A mock implementation of the <see cref="ILogger"/> interface for testing purposes.
    /// </summary>
    /// <remarks>This logger captures log messages in an in-memory collection and publishes them using the
    /// provided <see cref="IPublisher"/>. It can be used to verify logging behavior in unit tests by inspecting the
    /// captured messages.</remarks>
    /// <remarks>
    /// Initializes a new instance of the <see cref="MockLogger"/> class with the specified log level and publisher.
    /// </remarks>
    /// <param name="logLevel">The minimum <see cref="LogLevel"/> at which log messages will be processed.</param>
    /// <param name="publisher">The <see cref="IPublisher"/> instance used to publish log messages. Cannot be <see langword="null"/>.</param>
    sealed public class MockLogger<T>(LogLevel logLevel, IPublisher publisher = null) : ILogger<T>
    {
        private readonly LogLevel _logLevel = logLevel;
        private readonly IPublisher _publisher = publisher ?? new ConsolePublisher(20);

        /// <summary>
        /// Gets the collection of messages.
        /// </summary>
        public IList<string> Messages { get; } = [];

        /// <summary>
        /// Determines whether the collection contains any message that includes the specified substring.
        /// </summary>
        /// <param name="message">The substring to search for within the messages. Cannot be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if at least one message in the collection contains the specified substring;
        /// otherwise, <see langword="false"/>.</returns>
        public bool Contains(string message) => Messages.Any(m => m.Contains(message));

        /// <summary>
        /// Begins a logical operation scope.
        /// </summary>
        /// <remarks>The scope can be used to group a set of operations together, providing contextual
        /// information that can be accessed during the lifetime of the scope. This is commonly used in logging
        /// frameworks to include additional context in log messages.</remarks>
        /// <typeparam name="TState">The type of the state to associate with the scope.</typeparam>
        /// <param name="state">The state to associate with the scope. This can be used to provide contextual information.</param>
        /// <returns>An <see cref="IDisposable"/> that ends the logical operation scope when disposed.</returns>
        public IDisposable BeginScope<TState>(TState state) => null!;

        /// <summary>
        /// Determines whether logging is enabled for the specified log level.
        /// </summary>
        /// <param name="logLevel">The log level to check.</param>
        /// <returns><see langword="true"/> if logging is enabled for the specified <paramref name="logLevel"/>;  otherwise, <see
        /// langword="false"/>.</returns>
        public bool IsEnabled(LogLevel logLevel) => logLevel >= _logLevel;

        /// <summary>
        /// Logs a formatted message at the specified log level.
        /// </summary>
        /// <remarks>The method logs the message only if the specified <paramref name="logLevel"/> is
        /// enabled. The formatted message is added to the internal message collection and written to the
        /// publisher.</remarks>
        /// <typeparam name="TState">The type of the state object to be logged.</typeparam>
        /// <param name="logLevel">The severity level of the log message.</param>
        /// <param name="eventId">The identifier for the event being logged.</param>
        /// <param name="state">The state object containing information to be logged.</param>
        /// <param name="exception">The exception related to the log entry, or <see langword="null"/> if no exception is associated.</param>
        /// <param name="formatter">A function that formats the <paramref name="state"/> and <paramref name="exception"/> into a log message
        /// string.</param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                var message = $"[{logLevel}] {formatter(state, exception)}";
                Messages.Add(message);
                _publisher.WriteLine(message);
            }
        }
    }
}
