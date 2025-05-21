using Core.Contracts;
using Core.Extensions;
using Core.Models;
using Core.Serializers;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Services
{
    /// <summary>
    /// Provides functionality for analyzing faults using a chat-based AI model.
    /// </summary>
    /// <remarks>
    /// The constructor for the <see cref="FaultAnalysisService"/> class.
    /// </remarks>
    /// <param name="httpClient">Resilient http client used to make requests to the AI service.</param>
    /// <param name="model">The AI model to use for this request</param>
    /// <param name="apiKey">The API key to use for accessing the AI services.</param>
    /// <param name="apiUrl">The URL for making the request.</param>
    public class FaultAnalysisService(HttpClient httpClient, string model, string apiKey, string apiUrl) : IFaultAnalysisService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FaultAnalysisService"/> class.
        /// </summary>
        internal HttpClient ServiceClient { get; set; } = httpClient.IsNullThrow();

        /// <summary>
        /// Gets or sets the API key used for authentication with the AI service.
        /// </summary>
        internal string ApiKey { get; set; } = apiKey.IsNullThrow();

        /// <summary>
        /// Gets or sets the URL of the API endpoint for the AI service.
        /// </summary>
        internal string ApiUrl { get; set; } = apiUrl.IsNullThrow();

        /// <summary>
        /// Gets or sets the model identifier used for the AI service.
        /// </summary>
        internal string Model { get; set; } = model.IsNullThrow();

        /// <summary>
        /// Gets or sets the collection of messages associated with the current fault analysis context.
        /// </summary>
        internal IList<Message> Messages { get; set; } = [];

        /// <summary>
        /// Gets the <see cref="JsonSerializerOptions"/> used for serialization and deserialization.
        /// </summary>
        internal JsonSerializerOptions Options { get; set; } = new JsonSerializerOptions
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        /// <summary>
        /// Fault analysis method that sends a fault description to the AI service and returns the analysis result.
        /// </summary>
        /// <param name="messages">A collection of messages providing context for the analysis. Cannot be <see langword="null"/>.</param>
        /// <returns>Task results of the request.</returns>
        public async Task<ChatCompletionResponse> AnalyzeFaultAsync(IList<Message> messages)
        {
            messages.IsNullThrow();
            var request = new ChatRequest
            {
                Model = Model,
                Messages = messages
            };

            // Will fail here if instance is null.
            var requestBody = JsonConvertService.Instance!.Serialize(request, Options);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, ApiUrl)
            {
                Headers =
                {
                    { "Authorization", $"Bearer {ApiKey}" }
                },
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };

            var response = await ServiceClient.SendAsync(httpRequestMessage);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.IsNullThrow();
            return JsonConvertService.Instance.Deserialize<ChatCompletionResponse>(responseBody);
        }
    }
}
