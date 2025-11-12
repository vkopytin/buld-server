using Account.Db;
using Errors;
using Microsoft.EntityFrameworkCore;
using Models;
using MongoDB.Driver.Linq;

namespace Services;

public class ArticlesService : IArticlesService
{
  private readonly MongoDbContext dbContext;

  public ArticlesService(MongoDbContext dbContext)
  {
    this.dbContext = dbContext;
  }

  public async Task<(ArticleModel[]? articles, ServiceError? err)> ListArticles(int from = 0, int limit = 20)
  {
    var query = from a in dbContext.Articles.AsEnumerable()
                join m in dbContext.ArticleBlocks
                  on a.MediaId equals m.Id into mleft
                from sub in mleft.DefaultIfEmpty()
                orderby a.CreatedAt descending
                select a;

    var articles = query.Skip(from).Take(limit).Select(a => a.ToModel()).ToArray();

    return (articles, null);
  }
}
