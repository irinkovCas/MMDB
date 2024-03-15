using System.Text.RegularExpressions;

namespace Movies.Application.Models;

using Entities;

public partial class Movie
{

    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int YearOfRelease { get; set; }
    public string Slug { get; set; }

    public float? Rating { get; set; }

    #region Navigation Properties

    public ICollection<Genre>? Genres { get; set; }
    public ICollection<Rating>? Ratings { get; init; }

    #endregion

    public string GenerateSlug()
    {
        var sluggedTitle = SlugRegex()
            .Replace(Title, string.Empty)
            .ToLower()
            .Replace(" ", "-");

        return $"{sluggedTitle}-{YearOfRelease}";
    }

    [GeneratedRegex("[^0-9A-Za-z _-]", RegexOptions.NonBacktracking, 5)]
    private static partial Regex SlugRegex();

}
