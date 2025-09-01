using Core.Configuration;
using Core.Contracts;
using Loggers.Application;
using Loggers.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Logger.UnitTests.Application;

sealed public class ApplicationLogFactoryTest
{
    private readonly IOptions<AiEventSettings> _settings = Options.Create(new AiEventSettings());
    private readonly Func<ILogEvent> _logEventFactory = () => Mock.Of<ILogEvent>();

    [Fact]
    public void Constructor_Throws_WhenSettingsIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new ApplicationLogFactory(null!, _logEventFactory));
    }

    [Fact]
    public void Constructor_Throws_WhenFactoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new ApplicationLogFactory(_settings, null!));
    }

    [Fact]
    public void AddProvider_Throws_WhenProviderIsNull()
    {
        var factory = new ApplicationLogFactory(_settings, _logEventFactory);
        Assert.Throws<ArgumentNullException>(() => factory.AddProvider(null!));
    }

    [Fact]
    public void AddProvider_AddsProvider_WhenNotAlreadyAdded()
    {
        var factory = new ApplicationLogFactory(_settings, _logEventFactory);
        var provider = new Mock<ILoggerProvider>().Object;

        factory.AddProvider(provider);

        Assert.Contains(provider, factory.Providers);
    }

    [Fact]
    public void AddProvider_DoesNotAddDuplicateProvider()
    {
        var factory = new ApplicationLogFactory(_settings, _logEventFactory);
        var provider = new Mock<ILoggerProvider>().Object;

        factory.AddProvider(provider);
        factory.AddProvider(provider);

        Assert.Single(factory.Providers, provider);
    }

    [Fact]
    public void CreateLogger_ReturnsLogger_ForCategory()
    {
        var factory = new ApplicationLogFactory(_settings, _logEventFactory);
        var logger = factory.CreateLogger("TestCategory");

        Assert.NotNull(logger);
        Assert.IsType<ApplicationLogger>(logger);
    }

    [Fact]
    public void CreateLogger_ReturnsSameLogger_ForSameCategory()
    {
        var factory = new ApplicationLogFactory(_settings, _logEventFactory);
        var logger1 = factory.CreateLogger("TestCategory");
        var logger2 = factory.CreateLogger("TestCategory");

        Assert.Same(logger1, logger2);
    }

    [Fact]
    public void Dispose_DisposesProvidersAndClearsCollections()
    {
        var factory = new ApplicationLogFactory(_settings, _logEventFactory);
        var providerMock = new Mock<ILoggerProvider>();
        factory.AddProvider(providerMock.Object);
        factory.CreateLogger("TestCategory");

        ((IDisposable)factory).Dispose();

        providerMock.Verify(p => p.Dispose(), Times.Once);
        Assert.Empty(factory.Providers);
        Assert.Empty(factory.Loggers);
    }

    [Fact]
    public void Constructor_Sets_Optional_Parameters()
    {
        var publisherMock = new Mock<IPublisher>().Object;
        var faultAnalysisServiceMock = new Mock<IFaultAnalysisService>().Object;

        var factory = new ApplicationLogFactory(_settings, _logEventFactory, publisherMock, faultAnalysisServiceMock);

        Assert.Equal(publisherMock, factory.Publisher);
        Assert.Equal(faultAnalysisServiceMock, factory.FaultAnalysisService);
    }
}