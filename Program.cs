using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var jwtSecretKey = builder.Configuration["JWT:SecretKey"] ?? throw new Exception("appsettings config error: JWT secret key is null");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new Exception("appsettings config error: JWT issues is not specified");

var apiCorsPolicy = "ApiCorsPolicy";
builder.Services.AddCors(options =>
{
  options.AddPolicy(name: apiCorsPolicy,
  builder =>
  {
    builder.AllowCredentials()
      .WithOrigins(
        "http://local-dev.azurewebsites.net:4200", "http://localhost:4200", "http://dev.local:4200",
        "https://local-dev.azurewebsites.net:4200", "https://localhost:4200", "https://dev.local:4200",
        "https://local-dev.azurewebsites.net", "https://localhost", "https://dev.local"
      )
      .AllowAnyHeader()
      .AllowAnyMethod()
      .WithExposedHeaders("Authorization")
      ;
  });
});
builder.Services.AddHttpClient<IdmAccessTokenAuthSchemeHandler>();
builder.Services.AddControllers();
builder.Services.AddAuthorization(options =>
{
  var scopes = new[] {
    "read:data",
    "update:billing_settings",
    "read:customers",
    "read:files"
  };

  Array.ForEach(scopes, scope =>
    options.AddPolicy(scope,
      policy => policy.Requirements.Add(
        new ScopeRequirement(jwtIssuer, scope)
      )
    )
  );
}).AddAuthentication("IdmAccessToken")
.AddScheme<IdmAccessTokenAuthSchemeOptions, IdmAccessTokenAuthSchemeHandler>(
      "IdmAccessToken",
      opts => { }
);
builder.Services.AddHealthChecks();
builder.Services.AddSingleton<IAuthorizationHandler, RequireScopeHandler>();

var app = builder.Build();

app.UsePathBase("/api");
app.UseCors(apiCorsPolicy);
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();