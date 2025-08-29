using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Infrastructure.Examine;

namespace UrlTracker.IntegrationTests.Utils;

/// <summary>
/// This class replaces <see cref="RebuildOnStartupHandler"/> because that class uses some unnecessary static properties that break integration tests
/// </summary>
public sealed class CustomRebuildOnStartupHandler(
    ISyncBootStateAccessor syncBootStateAccessor,
    IIndexRebuilder backgroundIndexRebuilder,
    IRuntimeState runtimeState,
    CustomRebuildOnStartupHandlerState state)
    : INotificationHandler<UmbracoRequestBeginNotification>
{
    // This method should be a copy of the method in the original handler,
    //    except using the singleton state object instead of static fields
    public void Handle(UmbracoRequestBeginNotification notification)
    {
        if (runtimeState.Level != RuntimeLevel.Run)
        {
            return;
        }

        LazyInitializer.EnsureInitialized(
            ref state._isReady,
            ref state._isReadSet,
            ref state._isReadyLock,
            () =>
            {
                SyncBootState bootState = syncBootStateAccessor.GetSyncBootState();

                backgroundIndexRebuilder.RebuildIndexes(
                    bootState != SyncBootState.ColdBoot,
                    TimeSpan.Zero);

                return true;
            });
    }
}
