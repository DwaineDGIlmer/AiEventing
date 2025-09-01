using Domain.Incident;

namespace Domain.UnitTests.Incident;

sealed public class InternalIssueTest
{
    [Fact]
    public void DefaultConstructor_SetsDefaultValues()
    {
        var issue = new InternalIssue();

        Assert.Equal(string.Empty, issue.Description);
        Assert.False(issue.Preventable);
    }

    [Fact]
    public void CanSetDescription()
    {
        var issue = new InternalIssue
        {
            Description = "Test description"
        };

        Assert.Equal("Test description", issue.Description);
    }

    [Fact]
    public void CanSetPreventable()
    {
        var issue = new InternalIssue
        {
            Preventable = true
        };

        Assert.True(issue.Preventable);
    }
}