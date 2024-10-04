using Microsoft.AspNetCore.Authorization;

namespace UrlTracker.IntegrationTests.Utils
{
    internal class TestAuthorizationHandler : AuthorizationHandler<TestRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TestRequirement requirement)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
