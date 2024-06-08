namespace Auth.Models;

public record AuthClient
(
  string? ClientId,
  string? ClientName,
  string? ClientSecret,
  string[] GrantType,
  string[] AllowedScopes,
  string? ClientUri,
  string? RedirectUri,
  bool IsActive
);
