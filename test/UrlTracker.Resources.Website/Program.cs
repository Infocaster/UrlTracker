using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;
using UrlTracker.Resources.Website.SystemFeatures.AutoLogin;

public class Program
{
    private static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.CreateUmbracoBuilder()
            .AddBackOffice()
            .AddWebsite()
            .AddDeliveryApi()
            .AddComposers()
#if DEBUG
                .AddAutoLogin()
#endif
            .Build();

        WebApplication app = builder.Build();

        await app.BootUmbracoAsync();


        app.UseUmbraco()
            .WithMiddleware(u =>
            {
                u.UseBackOffice();
                u.UseWebsite();
            })
            .WithEndpoints(u =>
            {
                u.UseBackOfficeEndpoints();
                u.UseWebsiteEndpoints();
            });

        await app.RunAsync();
    }
}