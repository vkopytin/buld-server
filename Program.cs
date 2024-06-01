using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UsePathBase("/api");
app.MapHealthChecks("/health");
app.Run();