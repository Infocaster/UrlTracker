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
            _redactionScoreService.CreateAndSave(Defaults.DatabaseSchema.RedactionScores.Media, 3, "Media file not found");
            _redactionScoreService.CreateAndSave(Defaults.DatabaseSchema.RedactionScores.TechnicalFile, 1, "Technical file not found");
            _redactionScoreService.CreateAndSave(Defaults.DatabaseSchema.RedactionScores.File, 2, "File not found");
            _redactionScoreService.CreateAndSave(Defaults.DatabaseSchema.RedactionScores.Page, 4, "Page not found");
        }
    }
}
