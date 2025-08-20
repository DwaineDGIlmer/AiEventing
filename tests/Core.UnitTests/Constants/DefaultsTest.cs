using Core.Constants;

namespace Core.UnitTests.Constants;

public class DefaultsTest
{
    [Fact]
    public void CsvMimeType_ShouldBeTextCsv()
    {
        Assert.Equal("text/csv", Defaults.CsvMimeType);
    }

    [Fact]
    public void JsonMimeType_ShouldBeApplicationJson()
    {
        Assert.Equal("application/json", Defaults.JsonMimeType);
    }


    [Fact]
    public void HttpTimeout_ShouldBe60()
    {
        Assert.Equal(60, Defaults.HttpTimeout);
    }


    [Fact]
    public void DurationOfBreak_ShouldBe30()
    {
        Assert.Equal(30, Defaults.DurationOfBreak);
    }

    [Fact]
    public void OpenAiModel_ShouldBeGpt35Turbo()
    {
        Assert.Equal("gpt-3.5-turbo", Defaults.OpenAiModel);
    }

    [Fact]
    public void OpenAiClientName_ShouldBeCorrect()
    {
        Assert.Equal("OpenAi_Resilent_Http_ClientName", Defaults.OpenAiClientName);
    }

    [Fact]
    public void OpenAiABaseAddress_ShouldBeCorrect()
    {
        Assert.Equal("https://api.openai.com", Defaults.OpenAiABaseAddress);
    }

    [Fact]
    public void OpenAiEndpoint_ShouldBeCorrect()
    {
        Assert.Equal("chat/completions", Defaults.OpenAiEndpoint);
    }

    [Fact]
    public void SerpApiClientName_ShouldBeCorrect()
    {
        Assert.Equal("SerpApi_Resilent_Http_ClientName", Defaults.SerpApiClientName);
    }

    [Fact]
    public void SerpApiBaseAddress_ShouldBeCorrect()
    {
        Assert.Equal("https://serpapi.com", Defaults.SerpApiBaseAddress);
    }

    [Fact]
    public void SearchEndpoint_ShouldBeCorrect()
    {
        Assert.Equal("/search.json", Defaults.SearchEndpoint);
    }

    [Fact]
    public void SerpApiQuery_ShouldBeDataEngineer()
    {
        Assert.Equal("Data Engineer", Defaults.SerpApiQuery);
    }

    [Fact]
    public void SerpApiLocation_ShouldBeCharlotteNC()
    {
        Assert.Equal("Charlotte, NC", Defaults.SerpApiLocation);
    }

    [Fact]
    public void FileCompanyProfileDirectory_ShouldBeCorrect()
    {
        Assert.Equal("app_data/company_profiles", Defaults.FileCompanyProfileDirectory);
    }

    [Fact]
    public void FileJobProfileDirectory_ShouldBeCorrect()
    {
        Assert.Equal("app_data/job_summaries", Defaults.FileJobProfileDirectory);
    }
}