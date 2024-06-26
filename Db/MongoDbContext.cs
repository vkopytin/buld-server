using Auth.Db.Models;
using Microsoft.EntityFrameworkCore;
using ModelService.Db;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Auth.Db
{
  public class MongoDbContext : DbContext
  {
    public DbSet<User> Users { get; init; }
    public DbSet<Client> AuthClients { get; init; }
    public DbSet<ArticleRecord> Articles { get; init; }

    public MongoDbContext(MongoClient client)
     : base(new DbContextOptionsBuilder<MongoDbContext>().UseMongoDB(client, "main").Options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
      modelBuilder.Entity<User>().ToCollection("users");
      modelBuilder.Entity<Client>().ToCollection("authClients");
      modelBuilder.Entity<ArticleRecord>().ToCollection("articles");
    }
  }
}