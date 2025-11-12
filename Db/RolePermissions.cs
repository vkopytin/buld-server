namespace Account.Db;

public enum RolePermissions
{
  None = 0,
  List = 1 << 0,   // 1
  Details = 1 << 1,   // 2
  Create = 1 << 2,   // 4
  Edit = 1 << 3, // 8
  Remove = 1 << 4, // 16
}
