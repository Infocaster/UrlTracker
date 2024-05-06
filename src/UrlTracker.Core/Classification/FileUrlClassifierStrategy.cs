using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Classification
{
    /// <summary>
    /// An implementation of <see cref="IUrlClassifierStrategy"/> that classifies urls on whether or not they are a file, based on the last path segment
    /// </summary>
    public class FileUrlClassifierStrategy
        : UrlClassifierStrategyBase
    {
        /// <inheritdoc />
        public FileUrlClassifierStrategy(IRedactionScoreService redactionScoreService, ILogger<FileUrlClassifierStrategy> logger)
            : base(redactionScoreService, logger)
        { }

        /// <summary>
        /// The key of the redaction score to return if matching succeeds
        /// </summary>
        protected virtual Guid RedactionScoreKey => Defaults.DatabaseSchema.RedactionScores.File;

        /// <inheritdoc />
        protected override Guid? DoClassify(Url url)
        {
            var lastSegment = url.Path?.Split('/', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            if (IsFile(lastSegment, url)) return RedactionScoreKey;

            return null;
        }

        /// <summary>
        /// Checks whether or not the incoming url is a file
        /// </summary>
        /// <param name="lastSegment">The last segment of the url path</param>
        /// <param name="url">The url to classify</param>
        /// <returns><see langword="true"/> if the url is a file, <see langword="false"/> otherwise</returns>
        protected virtual bool IsFile([NotNullWhen(true)]string? lastSegment, Url url)
        {
            if (lastSegment is null) return false;

            if (!lastSegment.Contains('.')) return false;

            return true;
        }
    }
}
