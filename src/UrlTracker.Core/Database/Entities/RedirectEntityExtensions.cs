using Umbraco.Cms.Core.Services;

namespace UrlTracker.Core.Database.Entities;

public static class RedirectEntityExtensions
{
    public static IRedirect MigrateContentTarget(this IRedirect redirect, IIdKeyMap idKeyMap)
    {
        if (redirect.Target.Strategy != Defaults.DatabaseSchema.RedirectTargetStrategies.Content) return redirect;
        var splittedValue = redirect.Target.Value.Split(';');
        if(!int.TryParse(splittedValue[0], out int contentId)) return redirect;

        var maybeGuid = idKeyMap.GetKeyForId(contentId, Umbraco.Cms.Core.Models.UmbracoObjectTypes.Document);
        if (!maybeGuid.Success) return redirect;
        splittedValue[0] = maybeGuid.Result.ToString();

        var targetValue = EntityStrategy.ContentTarget(string.Join(";", splittedValue));
        redirect.Target = targetValue;

        return redirect;
    }
}