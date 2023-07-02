﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Services;
using UrlTracker.Backoffice.UI.Controllers.Models.RedirectTarget;

namespace UrlTracker.Backoffice.UI.Controllers.RequestHandlers
{
    internal interface IRedirectTargetRequestHandler
    {
        ContentTargetResponse? GetContentTarget(GetContentTargetRequest request);
    }

    internal class RedirectTargetRequestHandler : IRedirectTargetRequestHandler
    {
        private readonly IContentService _contentService;

        public RedirectTargetRequestHandler(IContentService contentService)
        {
            _contentService = contentService;
        }

        public ContentTargetResponse? GetContentTarget(GetContentTargetRequest request)
        {
            var content = _contentService.GetById(request.Id!.Value);
            if (content is null || content.Trashed) return null;

            return new ContentTargetResponse(content.ContentType.Icon!, content.GetCultureName(request.Culture) ?? content.Name!);
        }
    }
}
