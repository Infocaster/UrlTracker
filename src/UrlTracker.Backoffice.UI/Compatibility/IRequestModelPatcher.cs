using System;
using UrlTracker.Backoffice.UI.Controllers.Models;

namespace UrlTracker.Backoffice.UI.Compatibility
{
    [Obsolete("This service is temporarily introduced to remain compatible with the existing frontend. Do not use.")]
    public interface IRequestModelPatcher
    {
        AddRedirectRequest Patch(AddRedirectRequest request);
    }
}