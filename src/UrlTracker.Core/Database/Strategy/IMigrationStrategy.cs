using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlTracker.Core.Database.Strategy
{
    public interface IMigrationStrategy
    {
        void DoMigration();
    }
}
