using Domain.Analysis;

namespace Domain.UnitTests.Analysis;

public class TechnicalSummaryTest
{
    [Fact]
    public void TechnicalSummary_SetProperties_ValuesAreSet()
    {
        var techSummary = new TechnicalSummary
        {
            TechnicalReason = "Database connection timeout",
            ExternalReferences = new List<ExternalReference>
        {
            new ExternalReference { Type = "Doc", Url = "http://example.com", Description = "Reference" }
        }
        };

        Assert.Equal("Database connection timeout", techSummary.TechnicalReason);
        Assert.Single(techSummary.ExternalReferences);
        Assert.Equal("Doc", techSummary.ExternalReferences[0].Type);
    }
}
