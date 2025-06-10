using Core.Extensions;
using Loggers.Contracts;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Logger.UnitTets, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c599f69c3dd3ec398aa236557324d13db0f01fe1619e95bd66ab4fbd53143b2e57470a9c156080f2e3b088da0a7d40ce549ed7d803bc7cfc904077dce8ea5262c4afc77594841fb916db84485db81dfa6ba6cba1d449c0cb8c6aafd42245221dc310fae03f9b18c258c7939cd293f01a9b1ab9c433a53b278022f02a46958797")]
namespace Loggers.Publishers;

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
