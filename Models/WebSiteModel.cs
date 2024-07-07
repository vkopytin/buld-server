namespace Models;

public record WebSiteModel(
  Guid Id,
  Guid? ParentId,
  DateTime CreatedAt,
  string Name,
  string HostName,
  string AltHostName
);
