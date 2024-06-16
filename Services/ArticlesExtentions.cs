using Auth.Db.Models;
using Auth.Models;
using Models;
using ModelService.Db;

namespace Services;

public static class ArticlesExtentions
{
  public static ArticleModel ToModel(this ArticleRecord record)
  {
    return new(
      Id: record.Id,
      Title: record.Title,
      Description: record.Description,
      CreatedAt: record.CreatedAt
    );
  }
}