using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Classification
{
    /// <summary>
    /// A base implementation that matches urls by specific file extensions
    /// </summary>
    public abstract class FileExtensionUrlClassifierStrategyBase
        : FileUrlClassifierStrategy
    {
        /// <inheritdoc />
        protected FileExtensionUrlClassifierStrategyBase(IRedactionScoreService redactionScoreService, ILogger<FileExtensionUrlClassifierStrategyBase> logger)
            : base(redactionScoreService, logger)
        { }

        /// <summary>
        /// The extensions that this classifier should match on
        /// </summary>
        protected abstract ISet<string> Extensions { get; }

        /// <inheritdoc />
        protected override bool IsFile(string? lastSegment, Url url)
        {
            if(!base.IsFile(lastSegment, url)) return false;

            return Extensions.Any(ex => lastSegment.EndsWith(ex));
        }
    }
}
