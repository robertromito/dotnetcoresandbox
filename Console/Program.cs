using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace dotnetcore
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureHostConfiguration(config =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("hostsettings.json");
                    config.AddEnvironmentVariables(prefix: "ASPNETCORE_");
                    config.AddCommandLine(args);
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IHostedService, HelloWorld>();
                })
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json");
                    config.AddJsonFile(
                        $"appsettings.{context.HostingEnvironment.EnvironmentName}.json", 
                        optional: false);
                })
                .Build();

            await host.RunAsync();
        }
    }

    class HelloWorld : IHostedService, IDisposable
    {
        public IConfiguration Config { get; }

        public HelloWorld(IConfiguration config) => this.Config = config;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"Hello, World! I am {Config["Name"]}");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Goodbye, World!");
            return Task.CompletedTask;
        }
        public void Dispose() => Console.WriteLine("I am disposing");
    }
}
