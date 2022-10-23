using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Core.Composing;

namespace UrlTracker.Backoffice.UI.Extensions
{
    /// <summary>
    /// A collection builder that maintains a collection of pages for the URL Tracker dashboard
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UrlTrackerDashboardPageCollectionBuilder
        : WeightedCollectionBuilderBase<UrlTrackerDashboardPageCollectionBuilder, UrlTrackerDashboardPageCollection, IUrlTrackerDashboardPage>
    {
        /// <inheritdoc />
        protected override UrlTrackerDashboardPageCollectionBuilder This => this;
    }

    /// <summary>
    /// When implemented, this type provides a collection of pages to show on the URL Tracker dashboard
    /// </summary>
    public interface IUrlTrackerDashboardPageCollection
    {
        /// <summary>
        /// When implemented, this method returns the collection of pages
        /// </summary>
        /// <returns>The collection of registered pages</returns>
        IEnumerable<IUrlTrackerDashboardPage> Get();
    }

    /// <summary>
    /// An implementation of <see cref="IUrlTrackerDashboardPageCollection" /> that is managed by an umbraco collection builder.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UrlTrackerDashboardPageCollection
        : BuilderCollectionBase<IUrlTrackerDashboardPage>, IUrlTrackerDashboardPageCollection
    {
        /// <inheritdoc />
        public UrlTrackerDashboardPageCollection(Func<IEnumerable<IUrlTrackerDashboardPage>> items)
            : base(items)
        { }

        /// <inheritdoc />
        public IEnumerable<IUrlTrackerDashboardPage> Get()
        {
            return this;
        }
    }
}
