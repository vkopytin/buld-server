using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace Account.Db.Records;

public class RoleRecord : BaseEntity<Guid>
{
  public string RoleName { get; set; } = "";
  public int Permissions { get; set; } = 0; // Value from enum RolePermissions
}
