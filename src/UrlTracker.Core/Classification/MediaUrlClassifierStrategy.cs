using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace UrlTracker.Core.Classification
{
    /// <summary>
    /// An implementation of <see cref="IUrlClassifierStrategy"/> that classifies image urls
    /// </summary>
    public class MediaUrlClassifierStrategy
        : FileExtensionUrlClassifierStrategyBase
    {
        /// <inheritdoc />
        public MediaUrlClassifierStrategy(IRedactionScoreService redactionScoreService, ILogger<MediaUrlClassifierStrategy> logger)
            : base(redactionScoreService, logger)
        {
            Extensions = new HashSet<string>
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".webp",
                ".webm",
                ".mp4",
                ".mp3",
                ".ico",
                ".gif"
            };
        }

        /// <inheritdoc />
        protected override ISet<string> Extensions { get; }

        /// <inheritdoc />
        protected override Guid RedactionScoreKey => Defaults.DatabaseSchema.RedactionScores.Media;
    }
}
