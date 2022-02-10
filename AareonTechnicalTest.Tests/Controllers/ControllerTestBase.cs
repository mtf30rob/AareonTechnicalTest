using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace AareonTechnicalTest.Tests.Controllers
{
    public abstract class ControllerTestBase : IClassFixture<CustomWebApplicationFactory<AareonTechnicalTest.Startup>>
    {
        protected readonly HttpClient _client;

        public ControllerTestBase(CustomWebApplicationFactory<AareonTechnicalTest.Startup> factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }
    }
}

