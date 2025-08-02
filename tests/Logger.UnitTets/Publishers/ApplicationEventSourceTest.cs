using Logger.UnitTests;
using Loggers.Publishers;
using System.Diagnostics.Tracing;
using static Logger.UnitTests.MockPublisher;

namespace Logger.UnitTets.Publishers;

public class ApplicationEventSourceTest : UnitTestsBase
{
    [Fact]
    public async Task WriteLine_IncrementsTotalEvents_WhenMessageIsValid()
    {
        // Arrange
        var eventWritten = false;
        void listenerAction(EventWrittenEventArgs eventArg)
        {
            if (eventArg.EventId == 100)
            {
                eventWritten = true;
            }
        }
        using (var listener = new MockEventListener(listenerAction))
        {
            listener.EnableEvents(EventSourcePublisher.Log, EventLevel.LogAlways, (EventKeywords)0x1);
            var eventSource = EventSourcePublisher.Log;
            eventSource.TotalEvents = 0;
            string message = "Test message";

            // Act
            await eventSource.WriteLine(message);

            // Assert
            Assert.Equal(1, eventSource.TotalEvents);
        }
        Assert.True(eventWritten, "Event was not written to the listener.");
    }

    [Fact]
    public async Task WriteLine_DoesNotIncrementTotalEvents_WhenMessageIsNullOrEmpty()
    {
        // Arrange
        var eventWritten = false;
        void listenerAction(EventWrittenEventArgs eventArg)
        {
            if (eventArg.EventId == 100)
            {
                eventWritten = true;
            }
        }
        using (var listener = new MockEventListener(listenerAction))
        {
            listener.EnableEvents(EventSourcePublisher.Log, EventLevel.LogAlways, (EventKeywords)0x1);
            var eventSource = EventSourcePublisher.Log;
            eventSource.TotalEvents = 0;

            // Act
            await eventSource.WriteLine(null!);
            await eventSource.WriteLine(string.Empty);

            // Assert
            Assert.Equal(0, eventSource.TotalEvents);
        }
        Assert.False(eventWritten, "Event was written to the listener when it should not have been.");
    }

    [Fact]
    public async Task Write_IncrementsTotalEvents_WhenMessageIsValid()
    {
        // Arrange
        var eventWritten = false;
        void listenerAction(EventWrittenEventArgs eventArg)
        {
            if (eventArg.EventId == 101)
            {
                eventWritten = true;
            }
        }
        using (var listener = new MockEventListener(listenerAction))
        {
            listener.EnableEvents(EventSourcePublisher.Log, EventLevel.LogAlways, (EventKeywords)0x1);
            var eventSource = EventSourcePublisher.Log;
            eventSource.TotalEvents = 0;
            string message = "Test message";

            // Act
            await eventSource.Write(message);
            Assert.Equal(1, eventSource.TotalEvents);
        }

        // Assert
        Assert.True(eventWritten, "Event was not written to the listener.");
    }

    [Fact]
    public async Task Write_DoesNotIncrementTotalEvents_WhenMessageIsNullOrEmpty()
    {
        // Arrange
        var eventWritten = false;
        void listenerAction(EventWrittenEventArgs eventArg)
        {
            if (eventArg.EventId == 101)
            {
                eventWritten = true;
            }
        }
        using (var listener = new MockEventListener(listenerAction))
        {
            listener.EnableEvents(EventSourcePublisher.Log, EventLevel.LogAlways, (EventKeywords)0x1);
            var eventSource = EventSourcePublisher.Log;
            eventSource.TotalEvents = 0;

            // Act
            await eventSource.Write(null!);
            await eventSource.Write(string.Empty);

            // Assert
            Assert.Equal(0, eventSource.TotalEvents);
        }
        Assert.False(eventWritten, "Event was written to the listener when it should not have been.");
    }
}