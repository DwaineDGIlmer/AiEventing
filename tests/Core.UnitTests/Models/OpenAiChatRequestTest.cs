using Core.Models;

namespace Core.UnitTests.Models;

public class OpenAiChatRequestTest
{
    [Fact]
    public void OpenAiChatRequest_DefaultValues_ShouldInitializeProperties()
    {
        var request = new OpenAiChatRequest();

        Assert.Equal(string.Empty, request.Model);
        Assert.NotNull(request.Messages);
        Assert.Empty(request.Messages);
    }

    [Fact]
    public void OpenAiChatRequest_SetModel_ShouldUpdateModel()
    {
        var request = new OpenAiChatRequest
        {
            Model = "gpt-4"
        };

        Assert.Equal("gpt-4", request.Model);
    }

    [Fact]
    public void OpenAiChatRequest_AddMessages_ShouldContainMessages()
    {
        var message1 = new OpenAiMessage { Role = "user", Content = "Hello" };
        var message2 = new OpenAiMessage { Role = "assistant", Content = "Hi there!" };

        var request = new OpenAiChatRequest
        {
            Messages = new List<OpenAiMessage> { message1, message2 }
        };

        Assert.Equal(2, request.Messages.Count);
        Assert.Equal("user", request.Messages[0].Role);
        Assert.Equal("Hello", request.Messages[0].Content);
        Assert.Equal("assistant", request.Messages[1].Role);
        Assert.Equal("Hi there!", request.Messages[1].Content);
    }

    [Fact]
    public void OpenAiMessage_DefaultValues_ShouldInitializeProperties()
    {
        var message = new OpenAiMessage();

        Assert.Equal(string.Empty, message.Role);
        Assert.Equal(string.Empty, message.Content);
    }

    [Fact]
    public void OpenAiMessage_SetProperties_ShouldUpdateValues()
    {
        var message = new OpenAiMessage
        {
            Role = "system",
            Content = "Welcome!"
        };

        Assert.Equal("system", message.Role);
        Assert.Equal("Welcome!", message.Content);
    }
}