namespace Movies.Application; 

public class MovieRating {
    public Guid MovieId { get; init; }
    public string Slug { get; init; }
    public float Rating { get; init; }
}