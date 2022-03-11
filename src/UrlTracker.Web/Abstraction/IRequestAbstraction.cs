using System;
using Microsoft.AspNetCore.Http;

namespace UrlTracker.Web.Abstraction
{
    public interface IRequestAbstraction
    {
        Uri GetReferrer(HttpRequest request);
    }
}