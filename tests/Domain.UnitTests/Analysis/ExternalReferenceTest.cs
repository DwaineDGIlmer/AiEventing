using Domain.Analysis;

namespace Domain.UnitTests.Analysis;

public class ExternalReferenceTest
{
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
