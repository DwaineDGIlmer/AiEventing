using Domain.Incident;

namespace Domain.UnitTests.Incident;

public sealed class CauseTest
{
    [Fact]
    public void DefaultConstructor_SetsDefaultValues()
    {
        var cause = new Cause();

        Assert.Equal(string.Empty, cause.WhoOrWhat);
        Assert.False(cause.Preventable);
    }

    [Fact]
    public void CanSetWhoOrWhatProperty()
    {
        var cause = new Cause();
        cause.WhoOrWhat = "Human Error";

        Assert.Equal("Human Error", cause.WhoOrWhat);
    }

    [Fact]
    public void CanSetPreventableProperty()
    {
        var cause = new Cause();
        cause.Preventable = true;

        Assert.True(cause.Preventable);
    }

    [Fact]
    public void CanSetBothProperties()
    {
        var cause = new Cause
        {
            WhoOrWhat = "System Failure",
            Preventable = false
        };

        Assert.Equal("System Failure", cause.WhoOrWhat);
        Assert.False(cause.Preventable);
    }
}