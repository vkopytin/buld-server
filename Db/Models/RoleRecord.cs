using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Account.Db.Records;

public class RoleRecord
{
  [Key]
  [BsonId]
  public string RoleName { get; set; } = "";
  public int Permissions { get; set; } = 0; // Value from enum RolePermissions
}
