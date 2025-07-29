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

    [Fact]
    public void NextActions_SetProperties_ValuesAreSet()
    {
        var nextActions = new NextActions
        {
            Description = "Contact vendor for support",
            TechnicalContacts = new List<Contact>
            {
                new Contact { Name = "Jane Doe", Email = "jane@example.com", Role = "Support Engineer" }
            }
        };

        Assert.Equal("Contact vendor for support", nextActions.Description);
        Assert.Single(nextActions.TechnicalContacts);
        Assert.Equal("Jane Doe", nextActions.TechnicalContacts[0].Name);
    }

    [Fact]
    public void Contact_SetProperties_ValuesAreSet()
    {
        var contact = new Contact
        {
            Name = "John Smith",
            Email = "john.smith@example.com",
            Role = "Developer"
        };

        Assert.Equal("John Smith", contact.Name);
        Assert.Equal("john.smith@example.com", contact.Email);
        Assert.Equal("Developer", contact.Role);
    }

    [Fact]
    public void ExternalReference_SetProperties_ValuesAreSet()
    {
        var reference = new ExternalReference
        {
            Type = "Vendor Documentation",
            Url = "https://docs.vendor.com",
            Description = "Troubleshooting steps"
        };

        Assert.Equal("Vendor Documentation", reference.Type);
        Assert.Equal("https://docs.vendor.com", reference.Url);
        Assert.Equal("Troubleshooting steps", reference.Description);
    }
}