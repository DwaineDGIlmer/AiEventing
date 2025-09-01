using Domain.Analysis;

namespace Domain.UnitTests.Analysis;

public sealed class KnownIssueTest
{
    [Fact]
    public void KnownIssue_SetProperties_ValuesAreSet()
    {
        var knownIssue = new KnownIssue
        {
            IsKnown = true,
            Details = "Known issue with API rate limits",
            References = new List<ExternalReference>
        {
            new ExternalReference { Type = "API Docs", Url = "http://api.com", Description = "API Rate Limit" }
        }
        };

        Assert.True(knownIssue.IsKnown);
        Assert.Equal("Known issue with API rate limits", knownIssue.Details);
        Assert.Single(knownIssue.References);
        Assert.Equal("API Docs", knownIssue.References[0].Type);
    }
}
