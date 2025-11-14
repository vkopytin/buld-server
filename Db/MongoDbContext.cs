using Account.Db.Records;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Account.Db;

public partial class MongoDbContext : DbContext
{
  public DbSet<UserRecord> Users { get; init; }
  public DbSet<RoleRecord> Roles { get; init; }
  public DbSet<ClientRecord> AuthClients { get; init; }
  public DbSet<ArticleRecord> Articles { get; init; }
  public DbSet<ArticleBlockRecord> ArticleBlocks { get; set; }
  public DbSet<WebSiteRecord> WebSites { get; init; }
  public DbSet<CategoryRecord> Categories { get; init; }
  public DbSet<WebSiteArticleRecord> WebSiteArticles { get; init; }
  public DbSet<SecurityGroupRecord> SecurityGroups { get; init; }

  public MongoDbContext(MongoClient client)
   : base(new DbContextOptionsBuilder<MongoDbContext>().UseMongoDB(client, "main").Options)
  {

  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.Entity<CategoryRecord>().ToCollection("categories");
    modelBuilder.Entity<UserRecord>().ToCollection("users");
    modelBuilder.Entity<RoleRecord>().ToCollection("roles");
    modelBuilder.Entity<ClientRecord>().ToCollection("authClients");
    modelBuilder.Entity<ArticleRecord>().ToCollection("articles");
    modelBuilder.Entity<WebSiteRecord>().ToCollection("webSites");
    modelBuilder.Entity<ArticleBlockRecord>().ToCollection("articleBlocks");
    modelBuilder.Entity<WebSiteArticleRecord>().ToCollection("webSiteArticles");
    modelBuilder.Entity<SecurityGroupRecord>().ToCollection("securityGroups");

    modelBuilder.Entity<ArticleRecord>().HasOne(a => a.Media);
    modelBuilder.Entity<RoleRecord>().HasIndex(r => r.RoleName).IsUnique();
  }
}
