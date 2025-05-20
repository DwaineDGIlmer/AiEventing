namespace Loggers.Publishers.Tests
{
    public class ConsolePublisherTest
    {
        [Fact]
        public async Task WriteLine_EnqueuesMessage()
        {
            await using var publisher = new ConsolePublisher(5);
            string message = "Test WriteLine";
            await publisher.WriteLine(message);

            Assert.Equal(1, publisher.TotalEvents);
        }

        [Fact]
        public async Task Write_EnqueuesMessage()
        {
            await using var publisher = new ConsolePublisher(5);
            string message = "Test Write";
            await publisher.Write(message);

            // Wait for the background worker to process the queue
            await Task.Delay(20);

            Assert.Equal(1, publisher.TotalEvents);
        }

        [Fact]
        public async Task BackgroundWorker_DequeuesWriteLine()
        {
            await using var publisher = new ConsolePublisher(5);
            string message = "Test WriteLine";
            await publisher.WriteLine(message);

            // Wait for the background worker to process the queue
            await Task.Delay(20);

            Assert.Equal(1, publisher.TotalEvents);
        }

        [Fact]
        public async Task BackgroundWorker_Does_Not_DequeuesWriteLine()
        {
            await using var publisher = new ConsolePublisher(5);
            string message = string.Empty;
            await publisher.WriteLine(message);

            await Task.Delay(20);

            Assert.Equal(0, publisher.TotalEvents);
        }


        [Fact]
        public async Task BackgroundWorker_Does_Not_DequeuesWrite()
        {
            await using var publisher = new ConsolePublisher(5);
            string message = string.Empty;
            await publisher.Write(message);

            await Task.Delay(20);

            Assert.Equal(0, publisher.TotalEvents);
        }
        [Fact]
        public async Task WriteLine_IgnoresNullOrEmpty()
        {
            var publisher = new ConsolePublisher(1);
            await publisher.WriteLine(null!);
            await publisher.WriteLine(string.Empty);

            // Allow background worker to process
            await Task.Delay(10);

            Assert.True(publisher.TotalEvents == 0);
            await publisher.DisposeAsync();
            Assert.True(publisher.IsDisposed);
        }

        [Fact]
        public async Task Write_IgnoresNullOrEmpty()
        {
            var publisher = new ConsolePublisher(1);
            await publisher.Write(null!);
            await publisher.Write(string.Empty);

            // Allow background worker to process
            await Task.Delay(10);

            Assert.True(publisher.TotalEvents == 0);
            await publisher.DisposeAsync();
            Assert.True(publisher.IsDisposed);
        }
    }
}