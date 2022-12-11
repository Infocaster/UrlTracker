using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UrlTracker.Core.Database.Migrations
{
    internal class M202212111209_PopulateRedactionScores
        : MigrationBase
    {
        private readonly IRedactionScoreService _redactionScoreService;

        public M202212111209_PopulateRedactionScores(IMigrationContext context, IRedactionScoreService redactionScoreService)
            : base(context)
        {
            _redactionScoreService = redactionScoreService;
        }

        protected override void Migrate()
        {
            _redactionScoreService.CreateAndSave(Defaults.DatabaseSchema.RedactionScores.Image, 3);
            _redactionScoreService.CreateAndSave(Defaults.DatabaseSchema.RedactionScores.TechnicalFile, 1);
            _redactionScoreService.CreateAndSave(Defaults.DatabaseSchema.RedactionScores.File, 2);
            _redactionScoreService.CreateAndSave(Defaults.DatabaseSchema.RedactionScores.Page, 4);
        }
    }
}
