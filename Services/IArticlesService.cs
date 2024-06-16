using Errors;
using Models;

namespace Services;

public interface IArticlesService
{
  Task<(ArticleModel[]? articles, ServiceError? err)> ListArticles(int from = 0, int limit = 20);
}
