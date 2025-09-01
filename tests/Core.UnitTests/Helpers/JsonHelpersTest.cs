using Core.Helpers;
using System.Text.Json;

namespace Core.UnitTests.Helpers;

public sealed class JsonHelpersTest
{
    private class TestModel
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    [Fact]
    public void ValidateStrict_ValidJson_DoesNotThrow()
    {
        var json = "{\"Name\":\"John\",\"Age\":30}";
        Exception ex = Record.Exception(() => JsonHelpers.ValidateStrict<TestModel>(json));
        Assert.Null(ex);
    }

    [Fact]
    public void ValidateStrict_UnknownProperty_ThrowsJsonException()
    {
        var json = "{\"Name\":\"John\",\"Age\":30,\"Extra\":\"value\"}";
        Assert.Throws<JsonException>(() => JsonHelpers.ValidateStrict<TestModel>(json));
    }

    [Fact]
    public void ValidateStrict_InvalidJson_ThrowsJsonException()
    {
        var json = "{\"Name\":\"John\",";
        Assert.Throws<JsonException>(() => JsonHelpers.ValidateStrict<TestModel>(json));
    }

    [Fact]
    public void ParseUrls_ValidUrls_ReturnsExternalReferences()
    {
        var urls = new List<string>
        {
            "https://example.com",
            "http://test.org"
        };
        var result = JsonHelpers.ParseUrls(urls);
        Assert.Equal(2, result.Count);
        Assert.All(result, r => Assert.Equal("URL", r.Type));
        Assert.Contains(result, r => r.Url == "https://example.com");
        Assert.Contains(result, r => r.Url == "http://test.org");
    }

    [Fact]
    public void ParseUrls_InvalidUrls_IgnoresInvalid()
    {
        var urls = new List<string>
        {
            "https://valid.com",
            "not-a-url",
            "ftp://ftp.com"
        };
        var result = JsonHelpers.ParseUrls(urls);
        Assert.Equal(2, result.Count);
        Assert.DoesNotContain(result, r => r.Url == "not-a-url");
    }

    [Theory]
    [InlineData("prefix {\"key\": \"value\"} suffix", "{\"key\": \"value\"}")]
    [InlineData("{\"a\":1}", "{\"a\":1}")]
    [InlineData("no braces here", "no braces here")]
    [InlineData("{incomplete", "{incomplete")]
    [InlineData("}wrong order{", "}wrong order{")]
    public void ExtractJson_ExtractsOrReturnsOriginal(string input, string expected)
    {
        var result = JsonHelpers.ExtractJson(input);
        Assert.Equal(expected, result);
    }
}