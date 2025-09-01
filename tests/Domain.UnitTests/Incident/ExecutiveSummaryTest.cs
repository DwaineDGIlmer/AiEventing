using Domain.Incident;

namespace Domain.UnitTests.Incident;

sealed public class ExecutiveSummaryTest
{
    [Fact]
    public void ExecutiveSummary_DefaultValues_AreSet()
    {
        var summary = new ExecutiveSummary();

        Assert.Equal(string.Empty, summary.Id);
        Assert.Equal(string.Empty, summary.Summary);
        Assert.Equal(string.Empty, summary.Application);
        Assert.Equal(string.Empty, summary.FailingComponent);
        Assert.Equal(string.Empty, summary.Severity);
        Assert.Equal(string.Empty, summary.HighLevelCause);
        Assert.NotNull(summary.InternalIssues);
        Assert.NotNull(summary.ExternalIssues);
        Assert.NotNull(summary.EnvironmentalFailures);
        Assert.NotNull(summary.Cause);
    }

    [Fact]
    public void ExecutiveSummary_SetProperties_ValuesAreSet()
    {
        var summary = new ExecutiveSummary
        {
            Id = "INC123",
            Summary = "Major outage",
            Application = "AppX",
            FailingComponent = "Database",
            Severity = "High",
            HighLevelCause = "Network failure",
            InternalIssues = new List<InternalIssue>
            {
                new InternalIssue { Description = "Config error", Preventable = true }
            },
            ExternalIssues = new List<ExternalIssue>
            {
                new ExternalIssue { Description = "Vendor API down", Type = "Vendor", Details = "API v2" }
            },
            EnvironmentalFailures = new List<EnvironmentalFailure>
            {
                new EnvironmentalFailure { Type = "Power Outage", Description = "Datacenter lost power" }
            },
            Cause = new Cause { WhoOrWhat = "Network team", Preventable = false }
        };

        Assert.Equal("INC123", summary.Id);
        Assert.Equal("Major outage", summary.Summary);
        Assert.Equal("AppX", summary.Application);
        Assert.Equal("Database", summary.FailingComponent);
        Assert.Equal("High", summary.Severity);
        Assert.Equal("Network failure", summary.HighLevelCause);
        Assert.Single(summary.InternalIssues);
        Assert.Single(summary.ExternalIssues);
        Assert.Single(summary.EnvironmentalFailures);
        Assert.Equal("Network team", summary.Cause.WhoOrWhat);
        Assert.False(summary.Cause.Preventable);
    }

    [Fact]
    public void InternalIssue_DefaultValues_AreSet()
    {
        var issue = new InternalIssue();
        Assert.Equal(string.Empty, issue.Description);
        Assert.False(issue.Preventable);
    }

    [Fact]
    public void ExternalIssue_DefaultValues_AreSet()
    {
        var issue = new ExternalIssue();
        Assert.Equal(string.Empty, issue.Description);
        Assert.Equal(string.Empty, issue.Type);
        Assert.Equal(string.Empty, issue.Details);
    }

    [Fact]
    public void EnvironmentalFailure_DefaultValues_AreSet()
    {
        var failure = new EnvironmentalFailure();
        Assert.Equal(string.Empty, failure.Type);
        Assert.Equal(string.Empty, failure.Description);
    }

    [Fact]
    public void Cause_DefaultValues_AreSet()
    {
        var cause = new Cause();
        Assert.Equal(string.Empty, cause.WhoOrWhat);
        Assert.False(cause.Preventable);
    }
}