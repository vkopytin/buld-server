namespace Models;

public record UserToSave
(
  string UserName,
  string? Password,
  string Role,
  string? Name,
  bool IsActive
);
