using System.Security.Claims;
using System.Threading.Tasks;
using Account.Db;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;
using Utils;

namespace Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class HomeController : ControllerBase
{
  private readonly IProfileService profile;
  private readonly IArticlesService articles;
  private readonly IWebSitesService webSites;

  public HomeController(IProfileService profile, IArticlesService articles, IWebSitesService webSites)
  {
    this.profile = profile;
    this.articles = articles;
    this.webSites = webSites;
  }

  [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
  [HttpGet]
  [ActionName("user-login")]
  public IActionResult UserLogin()
  {
    return Ok(new { test = "test" });
  }

  [Authorize(
    "read:files",
    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpGet]
  [ActionName("list-clients")]
  public async Task<IActionResult> ListClients()
  {
    var securityGroupId = User.GetOid();
    var (authClients, err) = await profile.ListClients(securityGroupId);
    if (authClients is null)
    {
      return BadRequest(err);
    }

    return Ok(authClients);
  }

  [Authorize(
    "read:files",
    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpGet]
  [ActionName("list-connected-apps")]
  public async Task<IActionResult> ListConnectedApps()
  {
    await Task.CompletedTask;

    return BadRequest("Not implemented");
  }

  [Authorize(
    "read:files",
    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpPost]
  [ActionName("create-client")]
  public async Task<IActionResult> CreateClient([FromBody] ClientToSave request)
  {
    var client = request.ToModel();
    var securityGroupId = User.GetOid();
    var (authClient, err) = await profile.AddClient(securityGroupId, client);
    if (authClient is null)
    {
      return BadRequest(err);
    }

    return StatusCode(StatusCodes.Status201Created, authClient);
  }

  [Authorize(
    "read:files",
    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpPut]
  [ActionName("save-client")]
  public async Task<IActionResult> SaveClient([FromBody] ClientToSave request)
  {
    var client = request.ToModel();
    var securityGroupId = User.GetOid();
    var (authClient, err) = await profile.SaveClient(securityGroupId, client);
    if (authClient is null)
    {
      return BadRequest(err);
    }

    return Ok(authClient);
  }

  [Authorize(
    "read:files",
    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpGet("{clientId}")]
  [ActionName("client")]
  public async Task<IActionResult> GetClient(string clientId)
  {
    // rough implementation
    // toDO: imrpove over relevant scope check
    var securityGroupId = this.User.FindFirst("oid")?.Value;
    var (client, err) = await profile.GetClient(clientId);

    if (client is null)
    {
      return NotFound(err);
    }

    if (string.IsNullOrEmpty(securityGroupId))
    {
      return Ok(client);
    }

    if (client.SecurityGroupId == securityGroupId)
    {
      return Ok(client);
    }

    return Forbid();
  }

  [Authorize(
  "read:files",
  AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpGet]
  [ActionName("list-users")]
  public async Task<IActionResult> ListUsers(int from = 0, int limit = 10)
  {
    var (authUsers, err) = await profile.ListUsers(from, limit);

    if (authUsers is null)
    {
      return BadRequest(err);
    }

    return Ok(authUsers);
  }

  [Authorize(
    "read:files",
    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpPost]
  [ActionName("create-user")]
  public async Task<IActionResult> CreateUser([FromBody] UserToSave request)
  {
    var user = request.ToModel();
    var (authUser, err) = await profile.AddUser(user);

    if (authUser is null)
    {
      return BadRequest(err);
    }

    return StatusCode(StatusCodes.Status201Created, authUser);
  }

  [Authorize(
    "read:files",
    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpPut]
  [ActionName("save-user")]
  public async Task<IActionResult> SaveUser([FromBody] UserToSave request)
  {
    var user = request.ToModel();

    var (authUser, err) = await profile.SaveUser(user);

    if (authUser is null)
    {
      return BadRequest(err);
    }

    return Ok(authUser);
  }

  [Authorize(
    "read:user-info",
    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpGet("{userId}")]
  [ActionName("user")]
  public async Task<IActionResult> GetUser(string userId)
  {
    var (user, err) = await profile.GetUser(userId);

    if (user is null)
    {
      return NotFound(err);
    }

    return Ok(user);
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpGet]
  [ActionName("index")]
  public IActionResult Index()
  {
    var claims = new Dictionary<string, Dictionary<string, string>>();

    foreach (var claim in User.Claims)
    {
      claims.Add(claim.Type, new Dictionary<string, string>()
      {
        ["value"] = claim.Value,
        ["Issuer"] = claim.Issuer,
      });
    }

    return Ok(claims);
  }

  [HttpGet]
  [ActionName("public")]
  public IActionResult Public()
  {
    return Ok(new { Test = "test" });
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpGet]
  [ActionName("list-articles")]
  public async Task<IActionResult> ListArticles(int from = 0, int limit = 20)
  {
    var (articles, err) = await this.articles.ListArticles(from, limit);

    if (articles is null)
    {
      return BadRequest(err);
    }

    return Ok(articles);
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpGet]
  [ActionName("list-websites")]
  public async Task<IActionResult> ListWebSites(int from = 0, int limit = 20)
  {
    var (webSites, err) = await this.webSites.ListWebSites(from, limit);

    if (webSites is null)
    {
      return BadRequest(err);
    }

    return Ok(webSites);
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpGet]
  [ActionName("list-permissions")]
  public async Task<IActionResult> ListPermissions(int from = 0, int limit = 10)
  {
    var (roles, err) = await profile.ListPermissions();

    if (roles is null)
    {
      return BadRequest(err);
    }

    return Ok(roles);
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpGet]
  [ActionName("list-roles")]
  public async Task<IActionResult> ListUserRolesAndPermissions()
  {
    var (roles, err) = await profile.ListRoles();
    if (roles is null)
    {
      return BadRequest(err);
    }

    return Ok(roles);
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpGet]
  [ActionName("list-workflow-resources")]
  public IActionResult ListWorkflowResources()
  {
    var resources = Enum.GetValues(typeof(WorkflowResource))
      .Cast<WorkflowResource>()
      .Select(r => new
      {
        Id = (int)r,
        Name = r.ToString()
      })
      .ToArray();

    return Ok(resources);
  }
}
