using Auth.Db.Records;
using Models;

namespace Services;

public static class ArticlesExtentions
{
  public static ArticleModel ToModel(this ArticleRecord record)
  {
    return new(
      Id: record.Id,
      Title: record.Title,
      Description: record.Description,
      CreatedAt: record.CreatedAt,
      MediaId: record.MediaId,
      Origin: record.Origin,
      Media: record.Media?.ToModel()
    );
  }
}
