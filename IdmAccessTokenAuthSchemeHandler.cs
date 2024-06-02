using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal class IdmAccessTokenAuthSchemeHandler : AuthenticationHandler<IdmAccessTokenAuthSchemeOptions>
{
  private readonly HttpClient httpClient;

  public IdmAccessTokenAuthSchemeHandler(
      IOptionsMonitor<IdmAccessTokenAuthSchemeOptions> options,
      ILoggerFactory logger,
      UrlEncoder encoder,
      HttpClient httpClient) : base(options, logger, encoder)
  {
    this.httpClient = httpClient;
  }

  protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    var requestMessage = new HttpRequestMessage
    {
      Method = HttpMethod.Get,
      RequestUri = new Uri("https://idm2.azurewebsites.net/api/Auth/user-info")
    };
    var at = this.Request.Headers.Authorization;
    var accessToken = $"{this.Request.Headers["Authorization"]}";
    if (accessToken.StartsWith("Bearer"))
    {
      accessToken = accessToken.Substring("Bearer ".Length).Trim();
    }
    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

    var response = await this.httpClient.SendAsync(requestMessage);
    if (response.IsSuccessStatusCode)
    {
      var inter = await response.Content.ReadAsStringAsync();
      var userInfo = JsonSerializer.Deserialize<Dictionary<string, string>>(inter);
      // Read the token from request headers/cookies
      // Check that it's a valid session, depending on your implementation

      // If the session is valid, return success:
      var claims = userInfo.Select(pair => new Claim(
        pair.Key, pair.Value, null, "https://idm2.azurewebsites.net"
      )).ToArray();
      var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Tokens"));
      var ticket = new AuthenticationTicket(principal, this.Scheme.Name);

      return AuthenticateResult.Success(ticket);
    }


    // If the token is missing or the session is invalid, return failure:
    return AuthenticateResult.Fail("Authentication failed");
  }
}
