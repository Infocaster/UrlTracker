﻿using System;
using Microsoft.AspNetCore.Http;
using Moq;

namespace UrlTracker.Resources.Testing.Mocks
{
    public class HttpContextMock
    {
        private bool _urlConfigured;

        public HttpContextMock(Uri? incomingUrl = null)
        {
            ContextMock = new Mock<HttpContext>();
            RequestMock = new Mock<HttpRequest>();
            ResponseMock = new Mock<HttpResponse>();

            ContextMock.Setup(obj => obj.Request).Returns(RequestMock.Object);
            ContextMock.Setup(obj => obj.Response).Returns(ResponseMock.Object);
            _urlConfigured = false;
            if (incomingUrl is not null)
            {
                SetupUrl(incomingUrl);
            }
        }

        public void SetupUrl(Uri incomingUrl)
        {
            if (_urlConfigured) throw new InvalidOperationException("Url has already been configured");
            if (!incomingUrl.IsAbsoluteUri) throw new ArgumentException("Incoming url must always be absolute");
            RequestMock.Setup(obj => obj.Host).Returns(new HostString(incomingUrl.Host, incomingUrl.Port));
            RequestMock.Setup(obj => obj.Scheme).Returns(incomingUrl.Scheme);
            RequestMock.Setup(obj => obj.Path).Returns(new PathString(incomingUrl.AbsolutePath));
            RequestMock.Setup(obj => obj.QueryString).Returns(new QueryString(incomingUrl.Query));
            _urlConfigured = true;
        }

        public HttpContext Context => ContextMock.Object;
        public HttpResponse Response => ResponseMock.Object;
        public HttpRequest Request => RequestMock.Object;

        public Mock<HttpContext> ContextMock { get; }
        public Mock<HttpRequest> RequestMock { get; }
        public Mock<HttpResponse> ResponseMock { get; }
    }
}
