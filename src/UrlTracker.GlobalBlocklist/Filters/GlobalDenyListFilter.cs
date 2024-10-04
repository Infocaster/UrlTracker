using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Umbraco.Cms.Core.Models;
using UrlTracker.GlobalBlocklist.Context;
using UrlTracker.GlobalBlocklist.Services;
using UrlTracker.Web.Processing;

namespace UrlTracker.GlobalBlocklist.Filters
{
    public class GlobalDenyListFilter : IClientErrorFilter
    {
        private readonly IDenyListContext _context;

        public GlobalDenyListFilter(IDenyListContext context)
        {
            _context = context;
        }

        public async ValueTask<bool> EvaluateCandidateAsync(HttpContext context)
        {
            var url = string.Concat(context.Request.Scheme, "://", context.Request.Host, context.Request.Path, context.Request.QueryString);
            
            var blockedItems = _context.List;

            foreach (var item in blockedItems)
            {
                if (url.Contains(item)) return false;
            }

            return true;
        }
    }
}
