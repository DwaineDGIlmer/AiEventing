using Domain.Analysis;

namespace Domain.UnitTests.Analysis;

sealed public class NextActionsTest
{
    [Fact]
    public void NextActions_SetProperties_ValuesAreSet()
    {
        var nextActions = new NextActions
        {
            Description = "Contact vendor for support"
        };

        Assert.Equal("Contact vendor for support", nextActions.Description);
    }
}
