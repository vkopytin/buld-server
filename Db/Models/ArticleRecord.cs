using System.ComponentModel.DataAnnotations.Schema;
using Repository;

namespace ModelService.Db
{
  [Table("Article")]
  public class ArticleRecord : BaseEntity<Guid>
  {
    public string? Title { get; set; }
    public string? Description { get; set; }

    public string? Origin { get; set; }

    public DateTime UpdatedAt { get; set; }
    public DateTime? RemovedAt { get; set; }

    [ForeignKey("Category")]
    public int? CategoryId { get; set; }
    public Dictionary<string, object>? Category { get; set; }

    public int MediaId { get; set; }
    public Dictionary<string, object>? Media { get; set; }
  }
}