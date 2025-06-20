using Core.Extensions;
using Loggers.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsoleApp;

internal class Program
{
    static async Task Main(string[] args)
    {

        var builder = Host.CreateEmptyApplicationBuilder(settings: null);
        builder.Services
            .AddMcpServer()
            .WithStdioServerTransport()
            .WithToolsFromAssembly();

        await builder.Build().RunAsync();
    }
}