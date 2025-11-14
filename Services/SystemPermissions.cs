namespace Account.Db;

public class RolePermissionEntry
{
  public PermissionNames Name { get; set; }
  public RolePermissions Permissions { get; set; }
}

public static class SystemPermissions
{
  public static readonly RolePermissionEntry[] AllPermissions =
  [
    new() {
      Name = PermissionNames.list_users,
      Permissions = RolePermissions.List | RolePermissions.Details
    },
    new() {
      Name = PermissionNames.create_user,
      Permissions = RolePermissions.List | RolePermissions.Details | RolePermissions.Create
    },
    new() {
      Name = PermissionNames.viewall_user,
      Permissions = RolePermissions.Details | RolePermissions.All
    }
  ];
}
