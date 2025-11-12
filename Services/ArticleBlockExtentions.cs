using Account.Db.Records;
using Models;

namespace Services;

public static class ArticleBlockExtentions
{
  public static ArticleBlockModel ToModel(this ArticleBlockRecord record)
  {
    return new(
      Id: record.Id,
      Title: record.Title,
      Description: record.Description,
      Origin: record.Origin,
      SourceUrl: record.SourceUrl,
      FileName: record.FileName
    );
  }
}