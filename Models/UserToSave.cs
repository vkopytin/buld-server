namespace Models;

public record UserToSave
(
  string UserName,
  string? Name,
  string Role,
  bool IsActive
);
