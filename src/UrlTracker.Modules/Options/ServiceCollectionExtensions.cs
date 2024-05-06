using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace UrlTracker.Modules.Options
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Call this method to add a module to the collection. This collection is only for show and doesn't do anything.
        /// </summary>
        /// <param name="services">The DI container to register in</param>
        /// <param name="name">The name of your module</param>
        /// <param name="icon">An optional icon for your module</param>
        /// <returns>The DI container after the module has been registered</returns>
        public static IServiceCollection AddUrlTrackerModule(this IServiceCollection services, string name, string? icon = null)
            => services.Configure<UrlTrackerModuleOptions>(options => options.RegisterModule(name, icon));
    }
}
