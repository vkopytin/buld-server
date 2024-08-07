using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Repository;

namespace Auth.Db.Records;
public class ArticleBlockRecord : BaseEntity<int>
{
    public string? Title { get; set; }
    public string? Description { get; set; }

    public string? Origin { get; set; }

    public DateTime UpdatedAt { get; set; }

    #region Navigation Properties
    public Guid ArticleId { get; set; }

    [ForeignKey("ArticleId")]
    public ArticleRecord? Article { get; set; }

    #endregion

    #region Media Info
    public int MediaId { get; set; }
    [ForeignKey("MediaId")]
    public ArticleBlockRecord? Media { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public string? SourceUrl { get; set; }
    public string? FileName { get; set; }
    #endregion

    public string? Rank { get; set; }
}