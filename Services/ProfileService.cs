using Auth.Db;
using Auth.Models;
using Errors;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;

namespace Services;

public class ProfileService : IProfileService
{
  private readonly MongoDbContext dbContext;

  public ProfileService(MongoDbContext dbContext)
  {
    this.dbContext = dbContext;
  }

  public async Task<(AuthClient[]?, ProfileError?)> ListClients(int from = 0, int limit = 10)
  {
    var clients = await dbContext.AuthClients
      .Skip(from).Take(limit)
      .ToArrayAsync();

    return (clients.Select(c => c.ToModel()).ToArray(), null);
  }

  public async Task<(AuthClient?, ProfileError?)> GetClient(string clientId)
  {
    var client = await dbContext.AuthClients.Where(c => c.ClientId == clientId)
      .Take(1)
      .FirstOrDefaultAsync();

    if (client is null)
    {
      return (null, new(Message: $"Client with id: {clientId} doesn't exist"));
    }

    return (client.ToModel(), null);
  }

  public async Task<(AuthClient?, ProfileError?)> AddClient(AuthClient client)
  {
    var exists = await dbContext.AuthClients.AnyAsync(c => c.ClientId == client.ClientId);

    if (exists)
    {
      return (null, new(Message: "Can't create. Client with same id exists"));
    }

    await dbContext.AuthClients.AddAsync(client.ToDataModel());

    await dbContext.SaveChangesAsync();

    return (client, null);
  }

  public async Task<(AuthClient?, ProfileError?)> SaveClient(AuthClient client)
  {
    var existing = await dbContext.AuthClients.Where(c => c.ClientId == client.ClientId)
      .Take(1)
      .FirstOrDefaultAsync();

    if (existing is null)
    {
      return (null, new(Message: "Can't update client. Client doesn't exists."));
    }

    existing.ClientId = client.ClientId;
    existing.ClientName = client.ClientName;
    existing.ClientSecret = client.ClientSecret;
    existing.GrantType = client.GrantType;
    existing.AllowedScopes = client.AllowedScopes;
    existing.ClientUri = client.ClientUri;
    existing.RedirectUri = client.RedirectUri;
    existing.IsActive = client.IsActive;

    await dbContext.SaveChangesAsync();

    return (existing.ToModel(), null);
  }
}
