using System.Security.Claims;
using Account.Db;

namespace Utils;

public static class GenericExtentions
{
  public static string GetOid(this ClaimsPrincipal user)
  {
    var oid = user.FindFirst("oid")?.Value;
    if (string.IsNullOrEmpty(oid))
    {
      throw new ArgumentException("User does not have 'oid' claim");
    }

    return oid;
  }
}
