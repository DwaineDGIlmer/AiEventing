using Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HttpHarness.Controllers
{  
    /// <summary>
    /// Represents an API endpoint for interacting with ChatGPT functionality.
    /// </summary>
    /// <remarks>This controller provides operations related to the ChatGPT service, such as retrieving the
    /// status of the endpoint. It is designed to be used as part of an ASP.NET Core application and follows RESTful API
    /// conventions.</remarks>
    [ApiController]
    [Route("[controller]")]
    public class ChatGptEndpoint : ControllerBase
    {
        private readonly string _results = "{\"model\":\"gpt-3.5-turbo\",\"messages\":[{\"role\":\"system\",\"content\":\"You are a debugging assistant for .NET stack traces.\"},{\"role\":\"user\",\"content\":\"Analyze this stack trace and suggest causes or fixes:   at Extensions.ConsoleApp.Program.MethodB() in C:\\\\Users\\\\dwain\\\\source\\\\repos\\\\AiEventing\\\\src\\\\ConsoleApp\\\\Program.cs:line 57   at Extensions.ConsoleApp.Program.MethodA() in C:\\\\Users\\\\dwain\\\\source\\\\repos\\\\AiEventing\\\\src\\\\ConsoleApp\\\\Program.cs:line 52   at Extensions.ConsoleApp.Program.CauseException() in C:\\\\Users\\\\dwain\\\\source\\\\repos\\\\AiEventing\\\\src\\\\ConsoleApp\\\\Program.cs:line 47   at Extensions.ConsoleApp.Program.Main(String[] args) in C:\\\\Users\\\\dwain\\\\source\\\\repos\\\\AiEventing\\\\src\\\\ConsoleApp\\\\Program.cs:line 34\"}]}\r\n";
        private readonly ILogger<ChatGptEndpoint> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatGptEndpoint"/> class.
        /// </summary>
        /// <remarks>The provided <paramref name="logger"/> is required for logging purposes and must not
        /// be null.</remarks>
        /// <param name="logger">The logger instance used to log messages and events related to the <see cref="ChatGptEndpoint"/>.</param>
        public ChatGptEndpoint(ILogger<ChatGptEndpoint> logger)
        {
            _logger = logger;
        }

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
        [HttpPost(Name = "PostChatGptRequest")]
        public IActionResult PostChatGptRequest([FromBody] ChatRequest request)
        {
            // You can now access request.Model, request.Messages, etc.
            return Ok(JsonSerializer.Serialize(request));
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
        public IActionResult PostChatGptRequestWithResponse([FromBody] ChatRequest request)
        {            
            return Ok(_results);
        }
    }
}
