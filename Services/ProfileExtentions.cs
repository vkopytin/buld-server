using Auth.Db.Models;
using Auth.Models;
using Models;

namespace Services;

public static class ProfileExtentions
{
  public static AuthClient ToModel(this Client client)
  {
    return new AuthClient(
      client.ClientId,
      client.ClientName,
      client.ClientSecret,
      client.GrantType ?? [],
      client.AllowedScopes ?? [],
      client.ClientUri,
      client.RedirectUri,
      client.IsActive
    );
  }

  public static AuthClient ToModel(this ClientToSave request)
  {
    return new AuthClient
    (
      ClientId: request.ClientId,
      ClientName: request.ClientName,
      ClientSecret: request.ClientSecret,
      GrantType: request.GrantType,
      AllowedScopes: request.AllowedScopes,
      ClientUri: request.ClientUri,
      RedirectUri: request.RedirectUri,
      IsActive: request.IsActive
    );
  }

  public static Client ToDataModel(this AuthClient client)
  {
    return new()
    {
      ClientId = client.ClientId,
      ClientName = client.ClientName,
      ClientSecret = client.ClientSecret,
      GrantType = client.GrantType,
      AllowedScopes = client.AllowedScopes,
      ClientUri = client.ClientUri,
      RedirectUri = client.RedirectUri,
      IsActive = client.IsActive
    };
  }
}
