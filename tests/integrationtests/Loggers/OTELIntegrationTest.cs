using Core.Configuration;
using Core.Serializers;
using Loggers.Models;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace IntegrationTests.Loggers
{
    public class OTELIntegrationTest
    {
        public OTELIntegrationTest()
        {
            var settings = new AiEventSettings();
            if (!JsonConvertService.IsInitialized)
            {
                JsonConvertService.Initialize(new JsonSerializerOptions()
                {
                    WriteIndented = settings.WriteIndented,
                    DefaultIgnoreCondition = settings.DefaultIgnoreCondition,
                    Encoder = settings.UnsafeRelaxedJsonEscaping ? JavaScriptEncoder.UnsafeRelaxedJsonEscaping : null
                });
            }
        }

        [Fact]
        public void Serialize_WithException_IncludesExceptionFieldsAndStackTrace()
        {
            // Arrange: Throw and catch an exception to ensure stack trace is populated
            Exception ex;
            try
            {
                throw new InvalidOperationException("fail!");
            }
            catch (Exception caught)
            {
                ex = caught;
            }

            var logEvent = new OtelLogEvents
            {
                Exception = ex
            };

            // Act: Serialize to OTEL-compliant JSON
            var json = logEvent.Serialize();
            var doc = JsonNode.Parse(json)!;
            var attributes = doc["attributes"]!.AsObject();

            // Assert: Exception fields are present and correct
            Assert.Equal(typeof(InvalidOperationException).FullName, attributes["exception.type"]!.GetValue<string>());
            Assert.Equal("fail!", attributes["exception.message"]!.GetValue<string>());
            Assert.False(string.IsNullOrWhiteSpace(attributes["exception.stacktrace"]!.GetValue<string>()));
        }

        [Fact]
        public void Serialize_WithStackTrace_OverridesExceptionStackTrace()
        {
            // Arrange: Throw and catch an exception
            Exception ex;
            try
            {
                throw new InvalidOperationException("fail!");
            }
            catch (Exception caught)
            {
                ex = caught;
            }

            // Provide a custom stack trace
            var customStackTrace = new StackTrace();

            var logEvent = new OtelLogEvents
            {
                Exception = ex,
                StackTrace = customStackTrace
            };

            // Act: Serialize to OTEL-compliant JSON
            var json = logEvent.Serialize();
            var doc = JsonNode.Parse(json)!;
            var attributes = doc["attributes"]!.AsObject();

            // Assert: The stacktrace in attributes matches the custom stack trace
            Assert.Equal(customStackTrace.ToString(), attributes["exception.stacktrace"]!.GetValue<string>());
        }
    }
}
