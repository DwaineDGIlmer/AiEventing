using Core.Models;

namespace Core.UnitTests.Models;

public class AnalysisSummaryTest
{
    [Fact]
    public void AnalysisSummary_DefaultInitialization_PropertiesAreNotNull()
    {
        var summary = new AnalysisSummary();

        Assert.NotNull(summary.TechnicalSummary);
        Assert.NotNull(summary.KnownIssue);
        Assert.NotNull(summary.NextActions);
    }
}