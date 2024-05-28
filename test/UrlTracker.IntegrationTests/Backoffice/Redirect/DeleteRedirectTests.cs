namespace UrlTracker.IntegrationTests.Backoffice.Redirect
{
    public class DeleteRedirectTests : RedirectTestBase
    {
        private const string _endpoint = _endpointBase;

        [TestCase(TestName = "Delete deletes the redirect if exists")]
        public async Task Delete_NormalFlow_DeletesRedirects()
        {
            // arrange
            var model = await CreateStandardRedirectAsync();

            // act
            var response = await WebsiteFactory.CreateStandardClient().PostAsync(_endpoint + "/" + model.Id + "/delete", null);
            response.EnsureSuccessStatusCode();
            var redirect = await GetRedirectService().GetAsync(model.Id!.Value);

            // assert
            Assert.That(redirect, Is.Null);
        }
    }
}
