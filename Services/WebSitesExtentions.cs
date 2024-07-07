using Auth.Db.Records;
using Auth.Models;
using Models;

namespace Services;

public static class WebSitesExtentions
{
  public static WebSiteModel ToModel(this WebSiteRecord webSite)
  {
    return new(
      Id: webSite.Id,
      ParentId: webSite.ParentId,
      CreatedAt: webSite.CreatedAt,
      Name: webSite.Name ?? string.Empty,
      HostName: webSite.HostName ?? string.Empty,
      AltHostName: webSite.AltHostName ?? string.Empty
    );
  }
}
