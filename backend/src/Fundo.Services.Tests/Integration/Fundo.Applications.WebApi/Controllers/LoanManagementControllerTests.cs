using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Fundo.Services.Tests.Integration
{
    public class LoanManagementControllerTests : IClassFixture<WebApplicationFactory<Fundo.Applications.WebApi.Startup>>
    {
        private readonly HttpClient _client;

        public LoanManagementControllerTests(WebApplicationFactory<Fundo.Applications.WebApi.Startup> factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task GetAllLoans_WithoutAuth_ShouldReturnUnauthorized()
        {
            // Test without authentication should return 401 Unauthorized
            var response = await _client.GetAsync("/api/LoanManagement/loans");

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetSpecificLoan_WithoutAuth_ShouldReturnUnauthorized()
        {
            // Test without authentication should return 401 Unauthorized
            var response = await _client.GetAsync("/api/LoanManagement/loans/1");

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
