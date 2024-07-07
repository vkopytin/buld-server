using Auth.Db.Records;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Auth.Db
{
  public class MongoDbContext : DbContext
  {
    public DbSet<UserRecord> Users { get; init; }
    public DbSet<ClientRecord> AuthClients { get; init; }
    public DbSet<ArticleRecord> Articles { get; init; }
    public DbSet<ArticleBlockRecord> ArticleBlocks { get; set; }
    public DbSet<WebSiteRecord> WebSites { get; init; }
    public DbSet<CategoryRecord> Categories { get; init; }
    public DbSet<WebSiteArticleRecord> WebSiteArticles { get; init; }

    public MongoDbContext(MongoClient client)
     : base(new DbContextOptionsBuilder<MongoDbContext>().UseMongoDB(client, "main").Options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
      modelBuilder.Entity<CategoryRecord>().ToCollection("categories");
      modelBuilder.Entity<UserRecord>().ToCollection("users");
      modelBuilder.Entity<ClientRecord>().ToCollection("authClients");
      modelBuilder.Entity<ArticleRecord>().ToCollection("articles");
      modelBuilder.Entity<WebSiteRecord>().ToCollection("webSites");
      modelBuilder.Entity<ArticleBlockRecord>().ToCollection("articleBlocks");
      modelBuilder.Entity<WebSiteArticleRecord>().ToCollection("webSiteArticles");

      modelBuilder.Entity<ArticleRecord>().HasOne(a => a.Media);
    }
  }
}