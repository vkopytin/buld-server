using Auth.Models;
using Errors;

namespace Services;

public interface IProfileService
{
  Task<(AuthClient[]?, ProfileError?)> ListClients(int from = 0, int limit = 10);
  Task<(AuthClient?, ProfileError?)> GetClient(string clientId);
  Task<(AuthClient?, ProfileError?)> AddClient(AuthClient client);
  Task<(AuthClient?, ProfileError?)> SaveClient(AuthClient client);
}
