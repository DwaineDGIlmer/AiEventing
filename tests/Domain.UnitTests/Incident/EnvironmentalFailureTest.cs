using Domain.Incident;

namespace Domain.UnitTests.Incident;

public class EnvironmentalFailureTest
{
    [Fact]
    public void DefaultConstructor_ShouldInitializePropertiesToEmptyStrings()
    {
        var failure = new EnvironmentalFailure();

        Assert.Equal(string.Empty, failure.Type);
        Assert.Equal(string.Empty, failure.Description);
    }

    [Fact]
    public void Properties_ShouldSetAndGetValues()
    {
        var failure = new EnvironmentalFailure
        {
            Type = "Power Outage",
            Description = "Loss of electricity in building"
        };

        Assert.Equal("Power Outage", failure.Type);
        Assert.Equal("Loss of electricity in building", failure.Description);
    }
}