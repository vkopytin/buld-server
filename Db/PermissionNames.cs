using System.ComponentModel;

namespace Account.Db;

public enum PermissionNames : int
{
  [Description("none")]
  none,
  [Description("list_users")]
  list_users,
  [Description("create_user")]
  create_user,
  [Description("edit_user")]
  edit_user,
  [Description("remove_user")]
  remove_user,
  [Description("listall_users")]
  listall_users,
}
