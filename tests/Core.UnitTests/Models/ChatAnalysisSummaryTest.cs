using Core.Models;

namespace Core.UnitTests.Models;
sealed public class ChatAnalysisSummaryTest
{
    [Fact]
    public void DefaultConstructor_InitializesProperties()
    {
        var summary = new ChatAnalysisSummary();

        Assert.Equal(string.Empty, summary.TechnicalSummary);
        Assert.Equal(string.Empty, summary.KnownIssue);
        Assert.Equal(string.Empty, summary.NextActions);
        Assert.Equal(0, summary.ConfidenceScore);
        Assert.NotNull(summary.References);
        Assert.Empty(summary.References);
    }

    [Fact]
    public void AnalysisSummaryResult_ReturnsExpectedValues()
    {
        var summary = new ChatAnalysisSummary
        {
            TechnicalSummary = "Tech details",
            KnownIssue = "Known issue details",
            NextActions = "Do something",
            ConfidenceScore = 0.85m,
            References = ["https://example.com/doc1", "https://example.com/doc2"]
        };

        var result = summary.AnalysisSummaryResult;

        Assert.Equal("Tech details", result.TechnicalSummary.TechnicalReason);
        Assert.NotEmpty(result.TechnicalSummary.ExternalReferences);
        Assert.True(result.KnownIssue.IsKnown);
        Assert.Equal("Known issue details", result.KnownIssue.Details);
        Assert.NotEmpty(result.KnownIssue.References);
        Assert.Equal("Do something", result.NextActions.Description);
    }

    [Fact]
    public void AnalysisSummary_ReturnsExpectedValues()
    {
        var summary = new ChatAnalysisSummary
        {
            TechnicalSummary = "Tech summary",
            KnownIssue = "",
            NextActions = "Next steps",
            References = ["https://example.com/ref"]
        };

        var result = summary.AnalysisSummary;

        Assert.Equal("Tech summary", result.TechnicalSummary.TechnicalReason);
        Assert.False(result.KnownIssue.IsKnown);
        Assert.Equal("", result.KnownIssue.Details);
        Assert.NotEmpty(result.KnownIssue.References);
        Assert.Equal("Next steps", result.NextActions.Description);
    }
}