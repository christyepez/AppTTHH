using AppTTHH.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System.Net;

namespace AppTTHH.Tests;

public sealed class ApiIntegrationTests(HrApiFactory factory) : IClassFixture<HrApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Ready_ReturnsOk()
    {
        var response = await _client.GetAsync("/health/ready");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Employees_WithoutToken_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/hr/employees");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public void Oidc_authority_mode_configures_bearer_options()
    {
        using var oidcFactory = new HrApiFactory().WithWebHostBuilder(builder =>
        {
            builder.UseSetting("Jwt:Authority", "https://idp.corp.internal/");
            builder.UseSetting("Jwt:Audience", "portal-api");
            builder.UseSetting("Jwt:RequireHttpsMetadata", "true");
        });

        using var scope = oidcFactory.Services.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        Assert.Equal("https://idp.corp.internal/", options.Authority);
        Assert.Equal("portal-api", options.Audience);
        Assert.True(options.RequireHttpsMetadata);
        Assert.Null(options.TokenValidationParameters.IssuerSigningKey);
    }
}

public sealed class HrApiFactory : WebApplicationFactory<Program>
{
    public HrApiFactory()
    {
        Environment.SetEnvironmentVariable("Jwt__Issuer", null);
        Environment.SetEnvironmentVariable("Jwt__Audience", null);
        Environment.SetEnvironmentVariable("Jwt__Secret", null);
        Environment.SetEnvironmentVariable("JWT_ISSUER", "portal-corporativo");
        Environment.SetEnvironmentVariable("JWT_AUDIENCE", "portal-corporativo-clients");
        Environment.SetEnvironmentVariable("JWT_SECRET", "IntegrationTestSecret_AtLeast32Characters_Long!");
        Environment.SetEnvironmentVariable("ConnectionStrings__HrDb", "Server=test;Database=test;User Id=test;Password=test;");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<HrDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<HrDbContext>>();
            services.AddDbContext<HrDbContext>(options => options.UseInMemoryDatabase("apptthh-integration"));
        });
    }
}
