using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using Repository;

namespace Account.Db.Records;

[Table("WebSite")]
public class WebSiteRecord : BaseEntity<Guid>
{
  public Guid? ParentId { get; set; }
  public WebSiteRecord Parent { get; set; }
  public string? Name { get; set; }
  public string? HostName { get; set; }
  public string? AltHostName { get; set; }

  public ICollection<WebSiteRecord> SubSites { get; } = [];

  public ObjectId? UserId { get; set; }
  [ForeignKey("UserId")]
  public UserRecord? User { get; set; }

}
