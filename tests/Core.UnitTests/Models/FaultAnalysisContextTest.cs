using Core.Models;

namespace Core.UnitTests.Models;

public sealed class FaultAnalysisContextTest
{
    [Fact]
    public void DefaultConstructor_InitializesPropertiesWithDefaults()
    {
        var context = new FaultAnalysisContext();

        Assert.Equal(string.Empty, context.Id);
        Assert.Equal(string.Empty, context.Source);
        Assert.Equal(string.Empty, context.CreatedAt);
        Assert.Null(context.Workflows);
        Assert.NotNull(context.ExceptionContext);
        Assert.NotNull(context.CustomerContext);
    }

    [Fact]
    public void CanSetAndGetProperties()
    {
        var context = new FaultAnalysisContext
        {
            Id = "incident-123",
            Source = "SystemA",
            CreatedAt = "2024-06-01T12:00:00Z",
            Workflows = ["Workflow1", "Workflow2"],
            ExceptionContext = new ExceptionContext { Message = "Error occurred" },
            CustomerContext = new CustomerContext { Id = "cust-456" }
        };

        Assert.Equal("incident-123", context.Id);
        Assert.Equal("SystemA", context.Source);
        Assert.Equal("2024-06-01T12:00:00Z", context.CreatedAt);
        Assert.Equal(2, context.Workflows?.Count);
        Assert.Equal("Error occurred", context.ExceptionContext.Message);
        Assert.Equal("cust-456", context.CustomerContext.Id);
    }

    [Fact]
    public void Workflows_CanBeNullOrAssigned()
    {
        var context = new FaultAnalysisContext();

        Assert.Null(context.Workflows);

        context.Workflows = ["wf1"];
        Assert.Single(context.Workflows);
    }
}