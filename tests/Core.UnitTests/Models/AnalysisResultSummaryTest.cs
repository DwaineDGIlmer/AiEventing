using Core.Contracts;
using Core.Models;

namespace Core.UnitTests.Models;

public sealed class AnalysisResultSummaryTest
{
    private class FakeContactInformation
    {
        public string Email { get; set; } = "test@email.com";
    }

    private class FakeCustomerContext
    {
        public string CustomerId { get; set; } = "C123";
        public string CustomerName { get; set; } = "Test Customer";
        public FakeContactInformation ContactInformation { get; set; } = new FakeContactInformation();
    }

    private class FakeExceptionContext
    {
        public string ExceptionType { get; set; } = "TestFault";
    }

    private class FakeFaultContext
    {
        public FakeCustomerContext CustomerContext { get; set; } = new FakeCustomerContext();
        public FakeExceptionContext ExceptionContext { get; set; } = new FakeExceptionContext();
    }

    private class FakeIngestionRequest : IIngestionRequest
    {
        public string RequestId { get; set; } = "R001";
        public string Source { get; set; } = "UnitTest";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public IFaultAnalysisContext FaultContext { get; set; } = new FaultAnalysisContext();
    }

    [Fact]
    public void DefaultConstructor_InitializesProperties()
    {
        var summary = new AnalysisResultSummary();

        Assert.NotNull(summary);
        Assert.Equal(string.Empty, summary.Id);
        Assert.Equal(string.Empty, summary.CustomerId);
        Assert.Equal(string.Empty, summary.Source);
        Assert.Equal(string.Empty, summary.FaultType);
        Assert.True((DateTime.UtcNow - summary.Timestamp).TotalSeconds < 5);
        Assert.Null(summary.ProcessingErrors);
    }

    [Fact]
    public void Constructor_FromIngestionRequest_MapsPropertiesCorrectly()
    {
        var fakeRequest = new FakeIngestionRequest();
        var summary = new AnalysisResultSummary(fakeRequest);

        Assert.Equal(fakeRequest.RequestId, summary.Id);
        Assert.Equal(fakeRequest.FaultContext.CustomerContext.Id, summary.CustomerId);
        Assert.Equal(fakeRequest.Source, summary.Source);
        Assert.Equal(fakeRequest.FaultContext.ExceptionContext.ExceptionType, summary.FaultType);
        Assert.Equal(fakeRequest.Timestamp, summary.Timestamp);

        Assert.NotNull(summary.NextActions);
        Assert.NotNull(summary.NextActions.Description);
    }

    [Fact]
    public void ProcessingErrors_CanBeSetAndRetrieved()
    {
        var summary = new AnalysisResultSummary();
        var errors = new List<Error> { new() { ErrorMessage = "Test error" } };
        summary.ProcessingErrors = errors;

        Assert.Equal(errors, summary.ProcessingErrors);
    }
}