using System.Text.Json;

namespace Core.Models.Tests
{
    public class ChatRequestTest
    {
        [Fact]
        public void ChatRequest_DefaultConstructor_InitializesProperties()
        {
            var chatRequest = new ChatRequest();

            Assert.NotNull(chatRequest.Model);
            Assert.Equal(string.Empty, chatRequest.Model);
            Assert.NotNull(chatRequest.Messages);
            Assert.Empty(chatRequest.Messages);
        }

        [Fact]
        public void Message_DefaultConstructor_InitializesProperties()
        {
            var message = new Message();

            Assert.NotNull(message.Role);
            Assert.Equal(string.Empty, message.Role);
            Assert.NotNull(message.Content);
            Assert.Equal(string.Empty, message.Content);
        }

        [Fact]
        public void ChatRequest_CanSetAndGetProperties()
        {
            var messages = new List<Message>
            {
                new Message { Role = "user", Content = "Hello" },
                new Message { Role = "assistant", Content = "Hi there!" }
            };

            var chatRequest = new ChatRequest
            {
                Model = "gpt-4",
                Messages = messages
            };

            Assert.Equal("gpt-4", chatRequest.Model);
            Assert.Equal(2, chatRequest.Messages.Count);
            Assert.Equal("user", chatRequest.Messages[0].Role);
            Assert.Equal("Hello", chatRequest.Messages[0].Content);
        }

        [Fact]
        public void ChatRequest_SerializesToJson_Correctly()
        {
            var chatRequest = new ChatRequest
            {
                Model = "gpt-3.5-turbo",
                Messages = new List<Message>
                {
                    new Message { Role = "user", Content = "Test message" }
                }
            };

            var json = JsonSerializer.Serialize(chatRequest);

            Assert.Contains("\"model\":\"gpt-3.5-turbo\"", json);
            Assert.Contains("\"messages\":[", json);
            Assert.Contains("\"role\":\"user\"", json);
            Assert.Contains("\"content\":\"Test message\"", json);
        }

        [Fact]
        public void ChatRequest_DeserializesFromJson_Correctly()
        {
            var json = @"{
                ""model"": ""gpt-4"",
                ""messages"": [
                    { ""role"": ""system"", ""content"": ""You are a helpful assistant."" },
                    { ""role"": ""user"", ""content"": ""Hello!"" }
                ]
            }";

            var chatRequest = JsonSerializer.Deserialize<ChatRequest>(json);

            Assert.NotNull(chatRequest);
            Assert.Equal("gpt-4", chatRequest.Model);
            Assert.Equal(2, chatRequest.Messages.Count);
            Assert.Equal("system", chatRequest.Messages[0].Role);
            Assert.Equal("You are a helpful assistant.", chatRequest.Messages[0].Content);
            Assert.Equal("user", chatRequest.Messages[1].Role);
            Assert.Equal("Hello!", chatRequest.Messages[1].Content);
        }
    }
}