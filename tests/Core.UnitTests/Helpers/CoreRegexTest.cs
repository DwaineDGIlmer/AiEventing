using Core.Helpers;

namespace Core.UnitTests.Helpers;

sealed public class CoreRegexTest
{
    [Fact]
    public void SanitizeJson_ShouldHandleEmptyString()
    {
        var input = "";
        var expected = "";
        var result = CoreRegex.SanitizeJson(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void SanitizeJson_ShouldHandleWhitespaceOnly()
    {
        var input = "   \r\n   ";
        var expected = "";
        var result = CoreRegex.SanitizeJson(input);
        Assert.Equal(expected, result);
    }
}