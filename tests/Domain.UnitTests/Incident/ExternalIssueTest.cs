using Domain.Incident;

namespace Domain.UnitTests.Incident;

sealed public class ExternalIssueTest
{
    [Fact]
    public void DefaultConstructor_InitializesPropertiesToEmptyStrings()
    {
        var issue = new ExternalIssue();

        Assert.Equal(string.Empty, issue.Description);
        Assert.Equal(string.Empty, issue.Type);
        Assert.Equal(string.Empty, issue.Details);
    }

    [Fact]
    public void CanSetAndGetProperties()
    {
        var issue = new ExternalIssue
        {
            Description = "Network outage",
            Type = "Vendor",
            Details = "ISP is investigating"
        };

        Assert.Equal("Network outage", issue.Description);
        Assert.Equal("Vendor", issue.Type);
        Assert.Equal("ISP is investigating", issue.Details);
    }
}