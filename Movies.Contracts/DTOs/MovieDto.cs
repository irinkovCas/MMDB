namespace Movies.Contracts.DTOs;

public class MovieDto
{

    public required Guid Id { get; set; }

    public required string Title { get; set; }

    // public required string Slug { get; set; }
    public float? Rating { get; set; }

    public int? UserRating { get; set; }

    public required int YearOfRelease { get; set; }
    public required IEnumerable<string> Genres { get; set; } = Enumerable.Empty<string>();

}
