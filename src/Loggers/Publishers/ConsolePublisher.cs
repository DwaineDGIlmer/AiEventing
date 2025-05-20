using Core.Extensions;
using Core.Serializers;
using Loggers.Contracts;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Loggers.Publishers
{
    /// <summary>
    /// Provides an implementation of <see cref="IPublisher"/> that writes messages to the console output.
    /// </summary>
    sealed public class ConsolePublisher : IPublisher, IAsyncDisposable
    {
        /// <summary>
        /// A thread-safe queue that stores event messages to be written to the console.
        /// Each event is followed by a line terminator. The default line terminator is a 
        /// carriage return followed by a line feed (\r\n).
        /// </summary>
        internal ConcurrentQueue<string> WriteLineQueue { get; } = new();

        /// <summary>
        /// A thread-safe queue that stores event messages to be written to the console.
        /// </summary>
        internal ConcurrentQueue<string> WriteQueue { get; } = new();

        /// <summary>
        /// A thread-safe queue that stores log messages to be written to the console.
        /// </summary>
        internal Task? BackgroundWorker { get; set; }

        /// <summary>
        /// A static instance of <see cref="CancellationTokenSource"/> used to signal cancellation requests
        /// for operations managed by this class.
        /// </summary>
        internal CancellationTokenSource CTS = new();

        /// <summary>
        /// Gets the internal <see cref="ConsoleTraceListener"/> used for publishing messages to the console.
        /// </summary>
        internal ConsoleTraceListener Publisher { get; } = new ConsoleTraceListener();

        /// <summary>
        /// Gets or sets the delay, in milliseconds, used for the delay in milliseconds that the backgroud task will 
        /// wait before checking for a new event.
        /// </summary>
        internal int Delay { get; set; }

        /// <summary>
        /// Used primarily for testing.
        /// </summary>
        internal long TotalEvents { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsolePublisher"/> class, which publishes log events to the
        /// console.
        /// </summary>
        /// <remarks>This constructor starts a background task that continuously processes log events from
        /// internal queues and writes them to the console. The task runs until the associated cancellation token is
        /// triggered. Log events are serialized to JSON before being written.</remarks>
        /// <param name="delay">The delay, in milliseconds, between processing log events. If the value is less than or equal to 0, a
        /// default delay of 20 milliseconds is used.</param>
        public ConsolePublisher(int delay)
        {
            Delay = delay > 0 ? delay : 20;
            if (BackgroundWorker.IsNotNull())
                return;

            BackgroundWorker = Task.Run(async () =>
            {
                while (!CTS.Token.IsCancellationRequested)
                {
                    TotalEvents += WriteLineQueue.Count + WriteQueue.Count;
                    while (WriteLineQueue.TryDequeue(out var logEvent))
                    {
                        // Will fail if the instance is not initialized
                        Publisher.WriteLine(JsonConvertService.Instance!.Serialize(logEvent));
                    }
                    while (WriteQueue.TryDequeue(out var logEvent))
                    {
                        // Will fail if the instance is not initialized
                        Publisher.Write(JsonConvertService.Instance!.Serialize(logEvent));
                    }
                    await Task.Delay(Delay);
                }
            }, CTS.Token);
        }

        /// <summary>
        /// Writes a message followed by a line terminator to the console output.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public async Task WriteLine(string message)
        {
            if (message.IsNullOrEmpty())
                return;

            WriteLineQueue.Enqueue(message);
            await Task.Delay(1);
        }

        /// <summary>
        /// Writes a message to the console output without a line terminator.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public async Task Write(string message)
        {
            if (message.IsNullOrEmpty())
                return;

            WriteQueue.Enqueue(message);
            await Task.Delay(1);
        }

        /// <summary>
        /// Disposes the resources used by the <see cref="ConsolePublisher"/> instance.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (BackgroundWorker.IsNotNull())
            {
                CTS.Cancel();
                await BackgroundWorker!;
            }
            Publisher.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets a value indicating whether the publisher is disposed.
        /// </summary>
        internal bool IsDisposed => BackgroundWorker.IsNull() || BackgroundWorker.IsCompleted || BackgroundWorker.IsCanceled;
    }
}
