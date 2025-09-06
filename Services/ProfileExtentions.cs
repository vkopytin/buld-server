using Auth.Db.Records;
using Auth.Models;
using Models;
using MongoDB.Bson;

namespace Services;

public static class ProfileExtentions
{
  public static AuthClient ToModel(this ClientRecord client)
  {
    return new AuthClient(
      client.ClientId,
      client.ClientName,
      client.ClientSecret,
      client.SecurityGroupId.ToString(),
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
      SecurityGroupId: request.SecurityGroupId,
      GrantType: request.GrantType,
      AllowedScopes: request.AllowedScopes,
      ClientUri: request.ClientUri,
      RedirectUri: request.RedirectUri,
      IsActive: request.IsActive
    );
  }

  public static ClientRecord ToDataModel(this AuthClient client)
  {
    return new()
    {
      ClientId = client.ClientId,
      ClientName = client.ClientName,
      ClientSecret = client.ClientSecret,
      SecurityGroupId = client.SecurityGroupId is null ? null : ObjectId.Parse(client.SecurityGroupId),
      GrantType = client.GrantType,
      AllowedScopes = client.AllowedScopes,
      ClientUri = client.ClientUri,
      RedirectUri = client.RedirectUri,
      IsActive = client.IsActive
    };
  }

  public static AuthUser ToModel(this UserToSave user)
  {
    return new(
      UserName: user.UserName,
      Name: user.Name,
      Role: user.Role,
      IsActive: user.IsActive
    );
  }

  public static AuthUser ToModel(this UserRecord user)
  {
    return new(
      UserName: user.UserName,
      Name: user.Name,
      Role: user.Role,
      IsActive: user.IsActive
    );
  }

  public static UserRecord ToDataModel(this AuthUser user)
  {
    return new()
    {
      UserName = user.UserName,
      Name = user.Name ?? string.Empty,
      Role = user.Role,
      IsActive = user.IsActive
    };
  }
}
