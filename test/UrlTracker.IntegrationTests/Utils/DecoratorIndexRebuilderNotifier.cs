using Umbraco.Cms.Infrastructure.Examine;

namespace UrlTracker.IntegrationTests.Utils;

internal sealed class DecoratorIndexRebuilderNotifier(
    IIndexRebuilder decoratee,
    ExamineWaitContext waitContext)
        : IIndexRebuilder
{
    public bool CanRebuild(string indexName)
    {
        return decoratee.CanRebuild(indexName);
    }

    public void RebuildIndex(string indexName, TimeSpan? delay = null, bool useBackgroundThread = true)
    {
        decoratee.RebuildIndex(indexName, delay, false);
    }

    public void RebuildIndexes(bool onlyEmptyIndexes, TimeSpan? delay = null, bool useBackgroundThread = true)
    {
        // Force rebuilding of indexes on the current thread
        //    so that the integration tests can wait for the rebuild to complete.
        decoratee.RebuildIndexes(onlyEmptyIndexes, delay, false);

        // After rebuilding, send a signal to the wait context that the rebuild is done
        //    This way, the integration tests know that rebuilding is finished and testing can begin
        waitContext.Fire();
    }
}
