﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Core
{
    public static class RedactionScoreServiceExtensions
    {
        public static IRedactionScore CreateAndSave(this IRedactionScoreService service, Guid key, decimal score)
        {
            var entity = service.Create(key, score);
            service.Save(entity);

            return entity;
        }
    }
}
