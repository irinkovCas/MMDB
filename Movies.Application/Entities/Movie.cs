using System.Text.RegularExpressions;

namespace Movies.Application.Models;

public partial class Movie {

    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int YearOfRelease { get; set; }

    public string Slug => GenerateSlug();

    #region Navigation Properties

    public ICollection<Genre>? Genres { get; init; }
    public ICollection<Rating>? Ratings { get; init; }

    #endregion

    private string GenerateSlug() {
        var sluggedTitle = SlugRegex().Replace(Title, string.Empty)
            .ToLower().Replace(" ", "-");
        return $"{sluggedTitle}-{YearOfRelease}";
    }

    [GeneratedRegex("[^0-9A-Za-z _-]", RegexOptions.NonBacktracking, 5)]
    private static partial Regex SlugRegex();

}
