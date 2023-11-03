using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlTracker.Core.Database.Dtos;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Database.Factories
{
    internal static class ReferrerFactory
    {
        internal static ReferrerResponse Build(OccurrancesDto dto)
        {
            return new ReferrerResponse(dto.Occurrances, dto.Url);
        }
    }
}
