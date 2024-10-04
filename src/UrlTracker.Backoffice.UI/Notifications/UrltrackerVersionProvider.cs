using System.Reflection;

namespace UrlTracker.Backoffice.UI;

internal interface IUrltrackerVersionProvider
{
    public string GetCurrentVersion();
}


/// <summary>
/// Provides the urltracker version based on the assembly.
/// </summary>
internal class UrltrackerVersionProvider : IUrltrackerVersionProvider
{
    public string GetCurrentVersion()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        return version?.ToString() ?? "";
    }
}

