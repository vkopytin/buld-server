using Account.Db;
using Account.Db.Records;
using Auth.Models;
using Errors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Driver.Linq;

namespace Services;

public class ProfileService : IProfileService
{
  private readonly MongoDbContext dbContext;
  private readonly ILogger logger;

  public ProfileService(MongoDbContext dbContext, ILogger<ProfileService> logger)
  {
    this.dbContext = dbContext;
    this.logger = logger;
  }

  public async Task<(AuthClient[]?, ProfileError?)> ListClients(string securityGroupId, int from = 0, int limit = 10)
  {
    var securityGroupIdObjId = MongoDB.Bson.ObjectId.Parse(securityGroupId);
    var clients = await dbContext.AuthClients
      .Where(c => c.SecurityGroupId == securityGroupIdObjId)
      .Skip(from).Take(limit)
      .ToArrayAsync();

    return (clients.Select(c => c.ToModel()).ToArray(), null);
  }

  public async Task<(AuthClient?, ProfileError?)> GetClient(string clientId)
  {
    try
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
    catch (Exception ex)
    {
      this.logger.LogError(ex, "Error, while fetching clients from DB");
      return (null, new(Message: ex.Message));
    }
  }

  public async Task<(AuthClient?, ProfileError?)> AddClient(string securityGroupId, AuthClient client)
  {
    var exists = await dbContext.AuthClients.AnyAsync(c => c.ClientId == client.ClientId);

    if (exists)
    {
      return (null, new(Message: "Can't create. Client with same id exists"));
    }

    var record = client.ToDataModel();
    record.SecurityGroupId = MongoDB.Bson.ObjectId.Parse(securityGroupId);
    await dbContext.AuthClients.AddAsync(record);

    await dbContext.SaveChangesAsync();

    return (record.ToModel(), null);
  }

  public async Task<(AuthClient?, ProfileError?)> SaveClient(string securityGroupId, AuthClient client)
  {
    var existingClient = await dbContext.AuthClients.Where(
      c => c.ClientId == client.ClientId && c.SecurityGroupId == MongoDB.Bson.ObjectId.Parse(securityGroupId)
    ).Take(1).FirstOrDefaultAsync();

    if (existingClient is null)
    {
      return (null, new(Message: "Can't update client. Client doesn't exist."));
    }

    existingClient.ClientId = client.ClientId;
    existingClient.ClientName = client.ClientName;
    existingClient.ClientSecret = client.ClientSecret;
    existingClient.GrantType = client.GrantType;
    existingClient.AllowedScopes = client.AllowedScopes;
    existingClient.ClientUri = client.ClientUri;
    existingClient.RedirectUri = client.RedirectUri;
    existingClient.IsActive = client.IsActive;

    await dbContext.SaveChangesAsync();

    return (existingClient.ToModel(), null);
  }

  public async Task<(AuthUser[]?, ProfileError?)> ListUsers(int from = 0, int limit = 10)
  {
    var users = await dbContext.Users
      .Skip(from).Take(limit)
      .ToArrayAsync();

    return (users.Select(c => c.ToModel()).ToArray(), null);
  }

  public async Task<(AuthUser?, ProfileError?)> GetUser(string userName)
  {
    var user = await dbContext.Users.Where(c => c.UserName == userName)
      .Take(1)
      .FirstOrDefaultAsync();

    if (user is null)
    {
      return (null, new(Message: $"User with username: {userName} doesn't exist"));
    }

    return (user.ToModel(), null);
  }

  public async Task<(AuthUser?, ProfileError?)> AddUser(AuthUser user)
  {
    var exists = await dbContext.Users.AnyAsync(c => c.UserName == user.UserName);

    if (exists)
    {
      return (null, new(Message: "Can't create. User with same username exists"));
    }

    await dbContext.Users.AddAsync(user.ToDataModel());

    await dbContext.SaveChangesAsync();

    return (user, null);
  }

  public async Task<(AuthUser?, ProfileError?)> SaveUser(AuthUser user)
  {
    var existingUser = await dbContext.Users.Where(c => c.UserName == user.UserName)
      .Take(1)
      .FirstOrDefaultAsync();

    if (existingUser is null)
    {
      return (null, new(Message: "Can't update user. User doesn't exist."));
    }

    existingUser.UserName = user.UserName;
    existingUser.Name = user.Name ?? string.Empty;
    existingUser.Role = user.Role;
    existingUser.IsActive = user.IsActive;

    await dbContext.SaveChangesAsync();

    return (existingUser.ToModel(), null);
  }

  public Task<(object[], ProfileError?)> ListPermissions()
  {
    var roles = SystemPermissions.AllPermissions
    .Select(a => new
    {
      RoleName = a.Name.GetDescription(),
      Permissions = a.Permissions
    })
    .ToArray<object>();

    return Task.FromResult((roles, default(ProfileError)));
  }

  public async Task<(object[], ProfileError?)> ListRoles()
  {
    var roles = dbContext.Roles
    .Select(r => new
    {
      RoleName = r.RoleName,
      Resource = r.Resource,
      Permissions = (RolePermissions)r.Permissions
    })
    .ToArrayAsync();

    return (await roles as object[], default(ProfileError));
  }
}
