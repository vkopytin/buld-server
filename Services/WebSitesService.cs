using Auth.Db;
using Errors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;

namespace Services;

public class WebSitesService : IWebSitesService
{
  private readonly ILogger logger;
  private readonly MongoDbContext dbContext;

  public WebSitesService(MongoDbContext dbContext, ILogger<WebSitesService> logger)
  {
    this.dbContext = dbContext;
    this.logger = logger;
  }
  public async Task<(WebSiteModel[]? articles, ServiceError? err)> ListWebSites(int from = 0, int limit = 20)
  {
    try
    {
      var webSites = await dbContext.WebSites
        .OrderByDescending(a => a.CreatedAt)
        .Skip(from).Take(limit)
        .ToArrayAsync();

      return (webSites.Select(s => s.ToModel()).ToArray(), null);
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Error, listing webSites");
      return (null, new(Message: ex.Message));
    }
  }
}
