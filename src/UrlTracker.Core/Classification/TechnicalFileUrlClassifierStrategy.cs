using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace UrlTracker.Core.Classification
{
    /// <summary>
    /// An implementation of <see cref="IUrlClassifierStrategy"/> that classifies urls as technical files
    /// </summary>
    public class TechnicalFileUrlClassifierStrategy
        : FileExtensionUrlClassifierStrategyBase
    {
        /// <inheritdoc />
        public TechnicalFileUrlClassifierStrategy(IRedactionScoreService redactionScoreService, ILogger<TechnicalFileUrlClassifierStrategy> logger)
            : base(redactionScoreService, logger)
        {
            Extensions = new HashSet<string>
            {
                ".css",
                ".js",
                ".js.map",
                ".woff",
                ".ttf"
            };
        }

        /// <inheritdoc />
        protected override ISet<string> Extensions { get; }
        
        /// <inheritdoc />
        protected override Guid RedactionScoreKey => Defaults.DatabaseSchema.RedactionScores.TechnicalFile;
    }
}
