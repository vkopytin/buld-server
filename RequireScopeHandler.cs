using Microsoft.AspNetCore.Authorization;

public class RequireScopeHandler : AuthorizationHandler<ScopeRequirement>
{
  protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ScopeRequirement requirement)
  {
    // The scope must have originated from our issuer.
    var scopesClaim = context.User.FindFirst(c => c.Type == "scopes" && c.Issuer == requirement.Issuer);
    if (scopesClaim == null || string.IsNullOrEmpty(scopesClaim.Value))
      return Task.CompletedTask;

    // A token can contain multiple scopes and we need at least one exact match.
    if (scopesClaim.Value.Split(' ').Any(s => s == requirement.Scope))
      context.Succeed(requirement);
    return Task.CompletedTask;
  }
}
