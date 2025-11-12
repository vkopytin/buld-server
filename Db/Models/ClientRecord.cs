using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;

namespace Account.Db.Records;

public class ClientRecord
{
  [Key]
  public ObjectId Id { get; set; }
  public string? ClientId { get; set; }
  public string? ClientName { get; set; }
  public string? ClientSecret { get; set; }
  public MongoDB.Bson.ObjectId? SecurityGroupId { get; set; }
  public string[]? GrantType { get; set; }
  public string[]? AllowedScopes { get; set; }
  public string? ClientUri { get; set; }
  public string? RedirectUri { get; set; }
  public bool IsActive { get; set; }
}
