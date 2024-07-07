using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Auth.Db.Records;

public class UserRecord
{
  [Key]
  public ObjectId Id { get; set; }

  public string? UserName { get; set; }
  public string? Name { get; set; }
  public string? Role { get; set; }
  public bool IsActive { get; set; }
  public string? Password { get; set; }
}
