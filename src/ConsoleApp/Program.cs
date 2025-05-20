using Core.Extensions;
using Loggers.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Extensions.ConsoleApp;

internal class Program
{
    static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Register application services here
                services.InitializeServices(context.Configuration);
                services.InitializeLogging(context.Configuration);
            })
            .Build();

        // Run the application
        var logger = host.Services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Logging");

        logger.BeginScope("Scope 1");
        logger.LogInformation("Logging in scope 1");
        logger.BeginScope("Scope 2");
        logger.LogInformation("Logging in scope 2");

        try
        {
            CauseException();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "A test exception occurred");
        }

        Console.ReadKey();
    }

    static void CauseException()
    {
        // Simulate a call stack
        MethodA();
    }

    static void MethodA()
    {
        MethodB();
    }

    static void MethodB()
    {
        throw new InvalidOperationException("This is a test exception with stack frames.");
    }
}