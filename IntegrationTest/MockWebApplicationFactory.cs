using Infrastructure;
using IntegrationTest;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

public class MockWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
            })
            .AddScheme<AuthenticationSchemeOptions, FakeJwtBearerAuthenticationHandler>("Test", options => { });

            services.AddHttpContextAccessor();

            using (var serviceProvider = services.BuildServiceProvider())
            {
                IHttpContextAccessor accessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
                WorkContext.SetHttpContextAccessor(accessor);

            }
        });

       
    }
}
