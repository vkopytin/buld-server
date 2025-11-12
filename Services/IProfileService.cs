using Account.Db.Records;
using Auth.Models;
using Errors;

namespace Services;

public interface IProfileService
{
  Task<(AuthClient[]?, ProfileError?)> ListClients(string securityGroupId, int from = 0, int limit = 10);
  Task<(AuthClient?, ProfileError?)> GetClient(string clientId);
  Task<(AuthClient?, ProfileError?)> AddClient(string securityGroupId, AuthClient client);
  Task<(AuthClient?, ProfileError?)> SaveClient(string securityGroupId, AuthClient client);

  Task<(AuthUser[]?, ProfileError?)> ListUsers(int from = 0, int limit = 10);
  Task<(AuthUser?, ProfileError?)> GetUser(string userId);
  Task<(AuthUser?, ProfileError?)> AddUser(AuthUser user);
  Task<(AuthUser?, ProfileError?)> SaveUser(AuthUser user);
  Task<(RoleRecord[]?, ProfileError?)> ListRoles(int from = 0, int limit = 10);
}
