namespace UrlTracker.Core.Database.Models
{
    /// <summary>
    /// This type provides dials to influence the score of recommendations
    /// </summary>
    public class RecommendationScoreParameters
    {
        /// <summary>
        /// A factor that influences the importance of the variable score
        /// </summary>
        /// <remarks>
        /// A value close to 0 will make the variable score less important.
        /// A value far away from 0 will make the variable score more important.
        /// A value less than 0 will inverse the effect of the variable score.
        /// </remarks>
        public double VariableFactor { get; set; } = 1;

        /// <summary>
        /// A factor that influences the importance of the redaction score
        /// </summary>
        /// <remarks>
        /// A value close to 0 will make the redaction score less important.
        /// A value far away from 0 will make the redaction score more important.
        /// A value less than 0 will inverse the effect of the redaction score.
        /// </remarks>
        public double RedactionFactor { get; set; } = 1;

        /// <summary>
        /// A power that influences the dropoff speed over time.
        /// </summary>
        /// <remarks>
        /// <para>A value of 1 will make the dropoff constant (dt)</para>
        /// <para>A value of 0.5 will make the dropoff deccelerate over time (√dt)</para>
        /// <para>A value of 2 will make the dropoff accelerate over time (dt²)</para>
        /// </remarks>
        public double TimeFactor { get; set; } = 1;
    }
}