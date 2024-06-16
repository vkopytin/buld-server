using Auth.Db;
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
    var articles = await dbContext.Articles
      .OrderByDescending(a => a.CreatedAt)
      .Skip(from).Take(limit)
      .ToArrayAsync();

    return (articles.Select(a => a.ToModel()).ToArray(), null);
  }
}
