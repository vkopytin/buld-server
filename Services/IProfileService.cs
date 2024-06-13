using Auth.Models;
using Errors;

namespace Services;

public interface IProfileService
{
  Task<(AuthClient[]?, ProfileError?)> ListClients(int from = 0, int limit = 10);
  Task<(AuthClient?, ProfileError?)> GetClient(string clientId);
  Task<(AuthClient?, ProfileError?)> AddClient(AuthClient client);
  Task<(AuthClient?, ProfileError?)> SaveClient(AuthClient client);

  Task<(AuthUser[]?, ProfileError?)> ListUsers(int from = 0, int limit = 10);
  Task<(AuthUser?, ProfileError?)> GetUser(string userId);
  Task<(AuthUser?, ProfileError?)> AddUser(AuthUser user);
  Task<(AuthUser?, ProfileError?)> SaveUser(AuthUser user);
}
