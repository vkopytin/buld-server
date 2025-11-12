using System.ComponentModel.DataAnnotations;

namespace Account.Db.Records;

public class RoleRecord
{
  [Key]
  public string RoleName { get; set; } = "";
  public int Permissions { get; set; } = 0; // Value from enum RolePermissions
}
