using Core.Constants;

namespace Core.UnitTests.Constants;

public sealed class DefaultConstantsTest
{
    [Fact]
    public void CsvMimeType_ShouldBe_TextCsv()
    {
        Assert.Equal("text/csv", DefaultConstants.CsvMimeType);
    }

    [Fact]
    public void JsonMimeType_ShouldBe_ApplicationJson()
    {
        Assert.Equal("application/json", DefaultConstants.JsonMimeType);
    }

    [Fact]
    public void BaseAddress_ShouldBe_CorrectUrl()
    {
        Assert.Equal("http://192.168.100.5:6334/", DefaultConstants.BaseAddress);
    }

    [Fact]
    public void HttpClientName_ShouldBe_CorrectName()
    {
        Assert.Equal("VectorDb_Resilent_Http_ClientName", DefaultConstants.HttpClientName);
    }

    [Fact]
    public void HostAddress_ShouldBe_CorrectHost()
    {
        Assert.Equal("bb804cb7-02f8-4aba-af17-5d9158100958.us-east4-0.gcp.cloud.qdrant.io", DefaultConstants.HostAddress);
    }

    [Fact]
    public void ApiEndpoint_ShouldBe_CorrectEndpoint()
    {
        Assert.Equal("collections/rca_collection/points/search", DefaultConstants.ApiEndpoint);
    }

    [Fact]
    public void DefaultEmbedding_ShouldBe_TextEmbedding3Small()
    {
        Assert.Equal("text-embedding-3-small", DefaultConstants.DefaultEmbedding);
    }

    [Fact]
    public void ContentVersion_ShouldBe_One()
    {
        Assert.Equal(1, DefaultConstants.ContentVersion);
    }

    [Fact]
    public void RcaCollectionName_ShouldBe_RcaCollection()
    {
        Assert.Equal("rca_collection", DefaultConstants.RcaCollectionName);
    }

    [Fact]
    public void CollectionLimit_ShouldBe_Zero()
    {
        Assert.Equal((ulong)0, DefaultConstants.CollectionLimit);
    }

    [Fact]
    public void VECTOR_DB_QDRANT_CERT_THUMBPRINT_ShouldBe_ItsName()
    {
        Assert.Equal("VECTOR_DB_QDRANT_CERT_THUMBPRINT", DefaultConstants.VECTOR_DB_QDRANT_CERT_THUMBPRINT);
    }

    [Fact]
    public void QDRANT_LOCAL_SERVICE_API_KEY_ShouldBe_ItsName()
    {
        Assert.Equal("QDRANT_LOCAL_SERVICE_API_KEY", DefaultConstants.QDRANT_LOCAL_SERVICE_API_KEY);
    }

    [Fact]
    public void QDRANT_SERVICE_API_KEY_ShouldBe_ItsName()
    {
        Assert.Equal("QDRANT_SERVICE_API_KEY", DefaultConstants.QDRANT_SERVICE_API_KEY);
    }

    [Fact]
    public void AI_API_BASE_ADDRESS_ShouldBe_ItsName()
    {
        Assert.Equal("AI_API_BASE_ADDRESS", DefaultConstants.AI_API_BASE_ADDRESS);
    }

    [Fact]
    public void AI_API_ENDPOINT_ShouldBe_ItsName()
    {
        Assert.Equal("AI_API_ENDPOINT", DefaultConstants.AI_API_ENDPOINT);
    }

    [Fact]
    public void AI_MODEL_ShouldBe_ItsName()
    {
        Assert.Equal("AI_MODEL", DefaultConstants.AI_MODEL);
    }

    [Fact]
    public void OPENAI_API_KEY_ShouldBe_ItsName()
    {
        Assert.Equal("OPENAI_API_KEY", DefaultConstants.OPENAI_API_KEY);
    }

    [Fact]
    public void MCP_API_KEY_ShouldBe_ItsName()
    {
        Assert.Equal("MCP_API_KEY", DefaultConstants.MCP_API_KEY);
    }

    [Fact]
    public void RCASERVICE_API_KEY_ShouldBe_ItsName()
    {
        Assert.Equal("RCASERVICE_API_KEY", DefaultConstants.RCASERVICE_API_KEY);
    }

    [Fact]
    public void RCASERVICE_API_URL_ShouldBe_ItsName()
    {
        Assert.Equal("RCASERVICE_API_URL", DefaultConstants.RCASERVICE_API_URL);
    }
}