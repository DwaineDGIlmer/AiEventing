using Core.Configuration;
using Loggers.Application;
using Loggers.Contracts;
using Loggers.Publishers;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Loggers;

public class ApplicationLogProviderTest : UnitTestsBase
{
    [Fact]
    public void Constructor_UsesDefaultPublisher_WhenNoneProvided()
    {
        // Arrange
        var settings = new AiEventSettings { MinLogLevel = LogLevel.Debug };

        // Act
        var provider = new ApplicationLogProvider(settings, TestLogEventFactory);

        // Assert
        var publisher = provider.GetType().GetProperty("Publisher", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Public)!.GetValue(provider);
        Assert.NotNull(publisher);
        Assert.IsType<ConsolePublisher>(publisher);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenFactoryIsNull()
    {
        // Arrange
        var settings = new AiEventSettings { MinLogLevel = LogLevel.Information };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ApplicationLogProvider(settings, null!));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenSettingsIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ApplicationLogProvider(null!, TestLogEventFactory));
    }

    [Fact]
    public void CreateLogger_ReturnsLogger_WithInjectedPublisherAndScopeProvider()
    {
        // Arrange
        var settings = new AiEventSettings { MinLogLevel = LogLevel.Critical };
        var mockPublisher = new Mock<IPublisher>();
        var customScopeProvider = new LoggerExternalScopeProvider();
        var provider = new ApplicationLogProvider(settings, TestLogEventFactory, mockPublisher.Object);

        // Act
        var logger = provider.CreateLogger("CustomCategory");

        // Assert
        Assert.NotNull(logger);
        Assert.IsType<ApplicationLogger>(logger);
    }

    [Fact]
    public void Publisher_Property_Is_Set_When_Provided()
    {
        var settings = new AiEventSettings { MinLogLevel = LogLevel.Warning };
        var mockPublisher = new Mock<IPublisher>().Object;
        var provider = new ApplicationLogProvider(settings, TestLogEventFactory, mockPublisher);

        var publisher = provider.GetType().GetProperty("Publisher", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Public)!.GetValue(provider);
        Assert.Same(mockPublisher, publisher);
    }
}