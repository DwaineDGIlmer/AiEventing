using Domain.Fault;

namespace Domain.UnitTests.Fault;

public class FaultContextTest
{
    [Fact]
    public void DefaultConstructor_InitializesPropertiesWithDefaults()
    {
        var context = new FaultContext();

        Assert.Equal(string.Empty, context.Id);
        Assert.Equal(string.Empty, context.Source);
        Assert.Equal(string.Empty, context.CreatedAt);
        Assert.Null(context.Workflows);
    }

    [Fact]
    public void Properties_CanBeSetAndRetrieved()
    {
        var context = new FaultContext
        {
            Id = "123",
            Source = "TestSource",
            CreatedAt = "2024-06-01T12:00:00Z",
            Workflows = new List<string> { "wf1", "wf2" }
        };

        Assert.Equal("123", context.Id);
        Assert.Equal("TestSource", context.Source);
        Assert.Equal("2024-06-01T12:00:00Z", context.CreatedAt);
        Assert.NotNull(context.Workflows);
        Assert.Contains("wf1", context.Workflows);
        Assert.Contains("wf2", context.Workflows);
    }

    [Fact]
    public void Workflows_CanBeSetToNull()
    {
        var context = new FaultContext
        {
            Workflows = null
        };

        Assert.Null(context.Workflows);
    }
}