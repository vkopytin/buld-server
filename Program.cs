using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

IdentityModelEventSource.ShowPII = true;

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
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization(options =>
{
  var scopes = new[] {
    "read:user-info",
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
}).AddAuthentication(config =>
{
  config.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
  config.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
  // this is my Authorization Server Port
  options.Authority = jwtIssuer;
  options.ClientId = "platformnet6";
  options.ClientSecret = jwtSecretKey;
  options.ResponseType = "code";
  options.CallbackPath = "/signin-oidc";
  options.SaveTokens = true;
  options.UseSecurityTokenValidator = true;
  options.TokenValidationParameters = new TokenValidationParameters
  {
    SignatureValidator = delegate (string token, TokenValidationParameters validationParameters)
    {
      var jwt = new JwtSecurityToken(token);
      return jwt;
    },
  };
});
builder.Services.AddHealthChecks();
builder.Services.AddSingleton<IAuthorizationHandler, RequireScopeHandler>();

var app = builder.Build();
app.UsePathBase("/api");
app.UseCors(apiCorsPolicy);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();