namespace Models;

public record ArticleBlockModel
(
  int Id,
  string? Title,
  string? Description,
  string? Origin,
  string? SourceUrl,
  string? FileName
);
