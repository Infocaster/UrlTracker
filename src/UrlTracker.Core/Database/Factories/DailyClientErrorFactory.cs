using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlTracker.Core.Database.Dtos;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Database.Factories
{
    internal static class DailyClientErrorFactory
    {
        internal static DailyClientErrorResponse Build(DailyClientErrorDto dto)
        {
            return new DailyClientErrorResponse(dto.Date, dto.Occurrances);
        }
    }
}
