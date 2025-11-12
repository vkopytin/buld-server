using System;
using System.ComponentModel.DataAnnotations.Schema;
using Repository;

namespace Account.Db.Records;

[Table("Category")]
public class CategoryRecord : BaseEntity<int>
{
  public int? ParentId { get; set; }
  public CategoryRecord Parent { get; set; }
  public string Name { get; set; }
  public string AltName { get; set; }
  public string Description { get; set; }

  public ICollection<CategoryRecord> SubCategories { get; } = new List<CategoryRecord>();

  public Guid? WebSiteId { get; set; }
  public WebSiteRecord WebSite { get; set; }
}
