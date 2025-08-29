namespace UrlTracker.IntegrationTests.Utils;

// This is the context that is registered as a singleton
//    to keep track of the state of index rebuilding
public class CustomRebuildOnStartupHandlerState
{
    // The warnings here are explicitly disabled, because this is a workaround for an Umbraco issue
#pragma warning disable CA1051 // Do not declare visible instance fields
#pragma warning disable SA1401 // Fields should be private
    public bool _isReady;
    public bool _isReadSet;
    public object? _isReadyLock;
#pragma warning restore SA1401 // Fields should be private
#pragma warning restore CA1051 // Do not declare visible instance fields
}