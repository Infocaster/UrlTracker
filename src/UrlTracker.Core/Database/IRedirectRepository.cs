using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Persistence;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Core.Database
{
    public interface IRedirectRepository
        : IReadWriteQueryRepository<int, IRedirect>
    {
        void DeleteBulk(int[] ids);
        Task<IReadOnlyCollection<IRedirect>> GetAsync(IEnumerable<string> urlsAndPaths);
        Task<RedirectEntityCollection> GetAsync(uint skip, uint take, string? query, RedirectFilters filters, bool descending);
        Task<IReadOnlyCollection<IRedirect>> GetWithRegexAsync();
    }

    /// <summary>
    /// Filter type for listing redirects of specific types
    /// </summary>
    [Flags]
    public enum RedirectType
    {
        /// <summary>
        /// When no redirect type is defined, use none
        /// </summary>
        None = 0,

        /// <summary>
        /// A filter for temporary redirects
        /// </summary>
        Temporary = 1,

        /// <summary>
        /// A filter for permanent redirects
        /// </summary>
        Permanent = 2,

        /// <summary>
        /// A filter for all types of redirects. In practice, this works the same as <see cref="None"/>
        /// </summary>
        All = Temporary | Permanent
    }
}