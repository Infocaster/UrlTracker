using System;
using UrlTracker.Web.Controllers.Models;

namespace UrlTracker.Web.Compatibility
{
    [Obsolete("This service is temporarily introduced to remain compatible with the existing frontend. Do not use.")]
    public interface IRequestModelPatcher
    {
        AddRedirectRequest Patch(AddRedirectRequest request);
    }
}