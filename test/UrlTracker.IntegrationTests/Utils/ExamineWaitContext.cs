namespace UrlTracker.IntegrationTests.Utils;

public class ExamineWaitContext
{
    private TaskCompletionSource? _tcp;

    public Task SetAsync()
    {
        _tcp?.TrySetCanceled();
        _tcp = new TaskCompletionSource();

        // It is ok to ignore the warning here, because this process is well managed within the testing framework
#pragma warning disable VSTHRD003 // Avoid awaiting foreign Tasks
        return _tcp.Task;
#pragma warning restore VSTHRD003 // Avoid awaiting foreign Tasks
    }

    public void Fire()
    {
        _tcp?.TrySetResult();
    }
}