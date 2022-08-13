using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace UrlTracker.Resources.Website
{
    public class Program
    {
        public static Task Main(string[] args)
            => CreateHostBuilder(args)
                .Build()
                .RunAsync();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(x => x.ClearProviders())
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}