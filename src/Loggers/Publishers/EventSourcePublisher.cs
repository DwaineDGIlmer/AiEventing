using Core.Extensions;
using Loggers.Contracts;
using System.Diagnostics.Tracing;

namespace Loggers.Publishers
{
    /// <summary>
    /// Provides an implementation of <see cref="IPublisher"/> that writes messages to an event source.
    /// </summary>
    [EventSource(Name = "ApplicationEventSource")]
    sealed public class EventSourcePublisher : EventSource, IPublisher
    {
        /// <summary>
        /// Singleton instance of the <see cref="EventSourcePublisher"/>.
        /// </summary>
        public static EventSourcePublisher Log { get; } = new EventSourcePublisher();

        /// <summary>
        /// Used primarily for testing.
        /// </summary>
        internal long TotalEvents { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventSourcePublisher"/> class.
        /// </summary>
        public EventSourcePublisher() { }

        /// <summary>
        /// Writes a message followed by a line terminator.
        /// </summary>
        /// <param name="message">The message to write.</param>
        [Event(100)]
        public async Task WriteLine(string message)
        {
            if (message.IsNullOrEmpty() || !IsEnabled())
                return;

            TotalEvents++;
            WriteEvent(100, message);
            await Task.Delay(1);
        }

        /// <summary>
        /// Writes a message without a line terminator.
        /// </summary>
        /// <param name="message">The message to write.</param>
        [Event(101)]
        new public async Task Write(string message)
        {
            if (message.IsNullOrEmpty() || !IsEnabled())
                return;

            TotalEvents++;

            // Remove CR and LF from the end of the message.
            WriteEvent(101, message.TrimEnd('\r', '\n'));
            await Task.Delay(1);
        }
    }
}
