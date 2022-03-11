﻿using Umbraco.Cms.Core.Mapping;

namespace UrlTracker.Resources.Testing.Objects
{
    public class TestMapDefinition<TFrom, TTo> : IMapDefinition
    {
        public TTo To { get; set; }

        public void DefineMaps(IUmbracoMapper mapper)
        {
            mapper.Define<TFrom, TTo>((source, context) => To);
        }
    }
}
