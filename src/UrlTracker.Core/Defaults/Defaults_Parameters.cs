using System;
using UrlTracker.Core.Database.Models;

namespace UrlTracker.Core
{
    public static partial class Defaults
    {
        /// <summary>
        /// A static class to contain all default parameter values
        /// </summary>
        public static class Parameters
        {
            /// <summary>
            /// A default start date for search queries
            /// </summary>
            public static DateTime StartDate => new(1970, 1, 1);

            /// <summary>
            /// A default end date for search queries
            /// </summary>
            public static DateTime EndDate => DateTime.UtcNow;

            /// <summary>
            /// Default parameters for score calculation in recommendation queries
            /// </summary>
            public static RecommendationScoreParameters ScoreParameters = new();
        }
    }
}
