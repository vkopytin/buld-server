using Repository;

namespace Account.Db.Records;

public class RoleRecord : BaseEntity<Guid>
{
  public string RoleName { get; set; } = string.Empty;
  public WorkflowResource Resource { get; set; } = WorkflowResource.None;
  public int Permissions { get; set; }
}
