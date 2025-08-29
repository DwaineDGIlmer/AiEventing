using Microsoft.AspNetCore.Mvc;

namespace HttpHarness.Controllers
{
    /// <summary>
    /// Represents an API endpoint for interacting with ChatGPT functionality.
    /// </summary>
    /// <remarks>This controller provides operations related to the ChatGPT service, such as retrieving the
    /// status of the endpoint. It is designed to be used as part of an ASP.NET Core application and follows RESTful API
    /// conventions.</remarks>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ChatGptEndpoint"/> class.
    /// </remarks>
    /// <remarks>The provided <paramref name="logger"/> is required for logging purposes and must not
    /// be null.</remarks>
    /// <param name="logger">The logger instance used to log messages and events related to the <see cref="ChatGptEndpoint"/>.</param>
    [ApiController]
    [Route("[controller]")]
    public class ChatGptEndpoint(ILogger<ChatGptEndpoint> logger) : ControllerBase
    {
        //"{\"model\":\"gpt-3.5-turbo\",\"messages\":[{\"role\":\"system\",\"content\":\"You are a debugging assistant for .NET stack traces.\"},{\"role\":\"user\",\"content\":\"Analyze this stack trace and suggest causes or fixes:   at Extensions.ConsoleApp.Program.MethodB() in C:\\\\Users\\\\dwain\\\\source\\\\repos\\\\AiEventing\\\\src\\\\ConsoleApp\\\\Program.cs:line 57   at Extensions.ConsoleApp.Program.MethodA() in C:\\\\Users\\\\dwain\\\\source\\\\repos\\\\AiEventing\\\\src\\\\ConsoleApp\\\\Program.cs:line 52   at Extensions.ConsoleApp.Program.CauseException() in C:\\\\Users\\\\dwain\\\\source\\\\repos\\\\AiEventing\\\\src\\\\ConsoleApp\\\\Program.cs:line 47   at Extensions.ConsoleApp.Program.Main(String[] args) in C:\\\\Users\\\\dwain\\\\source\\\\repos\\\\AiEventing\\\\src\\\\ConsoleApp\\\\Program.cs:line 34\"}]}\r\n";
        private readonly string _results = "{\r\n  \"id\": \"chatcmpl-BZGjyd7bc4VzDuwgeluoFLs3O9fkA\",\n  \"object\": \"chat.completion\",\n  \"created\": 1747745530,\n  \"model\": \"gpt-3.5-turbo-0125\",\n  \"choices\": [\n    {\n      \"index\": 0,\n      \"message\": {\n        \"role\": \"assistant\",\n        \"content\": \"From the provided stack trace, it looks like an exception is being thrown in the `CauseException` method called by `Main`. This exception then causes `MethodA` and `MethodB` to execute before the original exception is propagated up the call stack.\\n\\nTo troubleshoot this issue, you need to review the code at the specified lines in `Program.cs` to identify the specific exception being thrown and its cause. Here are some steps you can take to address this issue:\\n1. Check the implementation of the `CauseException` method at line 47. Look for any code that could potentially result in an exception.\\n2. Analyze the code in `MethodA` and `MethodB` (lines 52 and 57) to understand what actions are being performed there and how they could be related to the exception thrown in `CauseException`.\\n3. Ensure proper error handling techniques are in place to catch and handle exceptions appropriately in the code.\\n4. Consider adding try-catch blocks around the code where exceptions may occur to handle them gracefully and prevent them from propagating up the call stack.\\n\\nBy following these steps and analyzing the code in the mentioned locations, you should be able to identify and address the cause of the exception in your application.\",\r\n        \"refusal\": null,\n        \"annotations\": []\n      },\n      \"logprobs\": null,\n      \"finish_reason\": \"stop\"\n    }\n  ],\n  \"usage\": {\n    \"prompt_tokens\": 179,\n    \"completion_tokens\": 252,\n    \"total_tokens\": 431,\n    \"prompt_tokens_details\": {\n      \"cached_tokens\": 0,\n      \"audio_tokens\": 0\n    },\n    \"completion_tokens_details\": {\n      \"reasoning_tokens\": 0,\n      \"audio_tokens\": 0,\n      \"accepted_prediction_tokens\": 0,\n      \"rejected_prediction_tokens\": 0\n    }\n  },\n  \"service_tier\": \"default\",\n  \"system_fingerprint\": null\n}";
        private readonly ILogger<ChatGptEndpoint> _logger = logger;

        /// <summary>
        /// Retrieves the status of the ChatGPT endpoint.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> containing an HTTP 200 OK response with a message indicating that the ChatGPT
        /// endpoint is running.</returns>
        [HttpGet(Name = "GetStatus")]
        public IActionResult GetStatus()
        {
            return Ok("ChatGPT endpoint is running.");
        }

        /// <summary>
        /// Processes a request sent to the ChatGPT endpoint.
        /// </summary>
        /// <remarks>This method is designed to handle HTTP POST requests. The request body must be a
        /// valid string.</remarks>
        /// <param name="request">The request payload as a JSON-formatted string.</param>
        /// <returns>An <see cref="IActionResult"/> containing the response to the request.  Typically, this will include an HTTP
        /// 200 status code with the echoed request content.</returns>
        [HttpPost(Name = "with-bool")]
#pragma warning disable IDE0060 // Remove unused parameter
        public IActionResult PostChatGptRequest([FromBody] OpenAiChatRequest request)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // You can now access request.OpenAiModel, request.Messages, etc.
            return Ok(true);
        }

        /// <summary>
        /// Handles a POST request to process a chat request and returns a sample response.
        /// </summary>
        /// <remarks>This method simulates processing a chat request and returns a predefined response. 
        /// The response includes metadata such as an ID, creation timestamp, and a sample message from the
        /// assistant.</remarks>
        /// <param name="request">The chat request containing the model and other parameters for generating a response.</param>
        /// <returns>An <see cref="IActionResult"/> containing a serialized sample response for the chat request.</returns>
        [HttpPost("with-response", Name = "PostChatGptRequestWithResponse")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Needs to match the API")]
        public IActionResult PostChatGptRequestWithResponse([FromBody] OpenAiChatRequest request)
        {
            return Ok(_results);
        }
    }
}
