using System.Collections.Generic;
using System.Linq;
using UrlTracker.Backoffice.UI.Controllers.Models.Scoring;
using UrlTracker.Core;

namespace UrlTracker.Backoffice.UI.Controllers.RequestHandlers
{
    internal interface IScoringRequestHandler
    {
        ScoreParametersResponse GetScoreParameters();
        IEnumerable<RedactionScoreResponse> ListRedactionScores();
    }

    internal class ScoringRequestHandler : IScoringRequestHandler
    {
        private readonly IRedactionScoreService _redactionScoreService;

        public ScoringRequestHandler(IRedactionScoreService redactionScoreService)
        {
            _redactionScoreService = redactionScoreService;
        }

        public IEnumerable<RedactionScoreResponse> ListRedactionScores()
        {
            var scores = _redactionScoreService.GetAll();
            return scores.Select(s => new RedactionScoreResponse(s.Key, s.RedactionScore));
        }

        public ScoreParametersResponse GetScoreParameters()
        {
            var scoreParameters = Core.Defaults.Parameters.ScoreParameters;
            return new ScoreParametersResponse(
                scoreParameters.VariableFactor,
                scoreParameters.RedactionFactor,
                scoreParameters.TimeFactor);
        }
    }
}
