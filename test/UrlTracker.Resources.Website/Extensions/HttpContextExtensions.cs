using System.Net;
using Microsoft.AspNetCore.Http;

namespace UrlTracker.Resources.Website.Extensions
{
    public static class HttpContextExtensions
    {
        // inspired by: https://www.strathweb.com/2016/04/request-islocal-in-asp-net-core/
        public static bool IsLocal(this HttpRequest request)
        {
            // a request is local if the remote and local ip address are the same or if the remote ip address is loopback
            var connection = request.HttpContext.Connection;
            if (connection.RemoteIpAddress is not null)
            {
                if (connection.LocalIpAddress is not null)
                {
                    return connection.RemoteIpAddress.Equals(connection.LocalIpAddress);
                }

                return IPAddress.IsLoopback(connection.RemoteIpAddress);
            }

            return false;
        }
    }
}
