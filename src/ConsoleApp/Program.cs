using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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