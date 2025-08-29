using Microsoft.Extensions.Hosting;
using Umbraco.Cms.Infrastructure;

namespace UrlTracker.IntegrationTests.Utils;

internal sealed class SuspendScheduledPublishingHostedService
    : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Suspendable.ScheduledPublishing.Suspend();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}
