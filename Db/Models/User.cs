using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;

namespace Auth.Db.Models;

public class User
{
  [Key]
  public ObjectId Id { get; set; }

  public string? UserName { get; set; }
  public string? Name { get; set; }
  public string? Role { get; set; }
  public bool IsActive { get; set; }
  public string? Password { get; set; }
}
