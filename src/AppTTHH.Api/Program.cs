using System.Text;
using AppTTHH.Application;
using AppTTHH.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("Jwt:Secret is required");
var hrDbConnection = builder.Configuration.GetConnectionString("HrDb") ?? throw new InvalidOperationException("ConnectionStrings:HrDb is required");

builder.Services.AddControllers();
builder.Services.AddDbContext<HrDbContext>(o => o.UseSqlServer(hrDbConnection));
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o => o.TokenValidationParameters = new()
{
    ValidateIssuer = true, ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "portal-corporativo",
    ValidateAudience = true, ValidAudience = builder.Configuration["Jwt:Audience"] ?? "portal-corporativo-clients",
    ValidateLifetime = true, ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)), ClockSkew = TimeSpan.FromMinutes(1)
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(HrPermissions.EmployeesView, policy => policy.RequireAuthenticatedUser().RequireClaim(HrPermissions.ClaimType, HrPermissions.EmployeesView));
    options.AddPolicy(HrPermissions.EmployeesManage, policy => policy.RequireAuthenticatedUser().RequireClaim(HrPermissions.ClaimType, HrPermissions.EmployeesManage));
});

var app = builder.Build();
using (var scope = app.Services.CreateScope()) await scope.ServiceProvider.GetRequiredService<HrDbContext>().Database.EnsureCreatedAsync();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "Healthy", service = "AppTTHH" })).AllowAnonymous();
app.MapGet("/health/ready", async (HrDbContext db) => await db.Database.CanConnectAsync() ? Results.Ok(new { status = "Ready" }) : Results.StatusCode(503)).AllowAnonymous();
app.Run();

public partial class Program;
