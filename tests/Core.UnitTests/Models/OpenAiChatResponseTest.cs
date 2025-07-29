using Core.Models;
using System.Text.Json;

namespace Core.UnitTests.Models;

public class OpenAiChatResponseTest
{
    [Fact]
    public void OpenAiChatResponse_DefaultValues_AreSet()
    {
        var response = new OpenAiChatResponse();

        Assert.NotNull(response.Id);
        Assert.NotNull(response.Object);
        Assert.NotNull(response.Model);
        Assert.NotNull(response.Choices);
        Assert.NotNull(response.Usage);
        Assert.Empty(response.Choices);
    }

    [Fact]
    public void CompletionChoice_DefaultValues_AreSet()
    {
        var choice = new CompletionChoice();

        Assert.Equal(0, choice.Index);
        Assert.NotNull(choice.Message);
        Assert.NotNull(choice.FinishReason);
    }

    [Fact]
    public void Usage_DefaultValues_AreSet()
    {
        var usage = new Usage();

        Assert.Equal(0, usage.PromptTokens);
        Assert.Equal(0, usage.CompletionTokens);
        Assert.Equal(0, usage.TotalTokens);
        Assert.Null(usage.PromptTokensDetails);
        Assert.Null(usage.CompletionTokensDetails);
    }

    [Fact]
    public void TokenDetail_DefaultValues_AreSet()
    {
        var detail = new TokenDetail();

        Assert.NotNull(detail.Role);
        Assert.NotNull(detail.Content);
        Assert.Equal(0, detail.Tokens);
    }

    [Fact]
    public void OpenAiChatResponse_Serialization_Deserialization_Works()
    {
        var response = new OpenAiChatResponse
        {
            Id = "test-id",
            Object = "chat.completion",
            Created = 1234567890,
            Model = "gpt-4",
            Choices = new List<CompletionChoice>
            {
                new CompletionChoice
                {
                    Index = 1,
                    Message = new OpenAiMessage { Role = "assistant", Content = "Hello!" },
                    FinishReason = "stop"
                }
            },
            Usage = new Usage
            {
                PromptTokens = 10,
                CompletionTokens = 20,
                TotalTokens = 30,
                PromptTokensDetails = new Dictionary<string, int> { { "user", 5 } },
                CompletionTokensDetails = new Dictionary<string, int> { { "assistant", 15 } }
            }
        };

        var json = JsonSerializer.Serialize(response);
        var deserialized = JsonSerializer.Deserialize<OpenAiChatResponse>(json);

        Assert.Equal(response.Id, deserialized!.Id);
        Assert.Equal(response.Object, deserialized.Object);
        Assert.Equal(response.Created, deserialized.Created);
        Assert.Equal(response.Model, deserialized.Model);
        Assert.Single(deserialized.Choices);
        Assert.Equal("assistant", deserialized.Choices[0].Message.Role);
        Assert.Equal("Hello!", deserialized.Choices[0].Message.Content);
        Assert.Equal("stop", deserialized.Choices[0].FinishReason);
        Assert.Equal(10, deserialized.Usage.PromptTokens);
        Assert.Equal(20, deserialized.Usage.CompletionTokens);
        Assert.Equal(30, deserialized.Usage.TotalTokens);
        Assert.NotNull(deserialized.Usage.PromptTokensDetails);
        Assert.Equal(5, deserialized.Usage.PromptTokensDetails["user"]);
        Assert.NotNull(deserialized.Usage.CompletionTokensDetails);
        Assert.Equal(15, deserialized.Usage.CompletionTokensDetails["assistant"]);
    }
}