using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Classification
{
    /// <summary>
    /// A strategy pattern for converting urls into redaction scores
    /// </summary>
    public interface IUrlClassifierStrategy
    {
        /// <summary>
        /// When implemented, this method attempts to classify the incoming url and connect it to a redaction score
        /// </summary>
        /// <param name="url">The url to classify</param>
        /// <returns>A redaction score if classification was successful, else <see langword="null"/></returns>
        IRedactionScore? Classify(Url url);
    }

    /// <summary>
    /// A fallback classifier that returns a default redaction score in case no match was found
    /// </summary>
    public interface IFallbackUrlClassifier
    {
        /// <summary>
        /// When implemented, this method returns a fallback redaction score if no other match was found
        /// </summary>
        /// <param name="url">The url to classify</param>
        /// <returns>A redaction score</returns>
        IRedactionScore Classify(Url url);
    }
}
