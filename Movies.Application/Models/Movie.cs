using System.Text.RegularExpressions;

namespace Movies.Application; 

public partial class Movie {
    
    public required Guid Id { get; init; }
    public required string Title { get; set; }
    public string Slug => GenerateSlug();
    public required int YearOfRelease { get; set; }
    public int? UserRating { get; set; }
    public required List<string> Genres { get; init; } = new();
    public float? Rating { get; set; }

    // "The Matrix" => "the-matrix-1999
    private string GenerateSlug() {
        var sluggedTitle = SlugRegex().Replace(Title, String.Empty).ToLower().Replace(" ", "-");
        return $"{sluggedTitle}-{YearOfRelease}";
    }

    [GeneratedRegex("^[0-9A-Za-z _-]", RegexOptions.NonBacktracking, 5)]
    private static partial Regex SlugRegex();
}