using Errors;
using Models;

namespace Services;

public interface IWebSitesService
{
  Task<(WebSiteModel[]? articles, ServiceError? err)> ListWebSites(int from = 0, int limit = 20);
}
