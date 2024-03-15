namespace Movies.Contracts.Requests;

public class UpdateMovieRequest
{

    public string Title { get; set; } = string.Empty;
    public int YearOfRelease { get; set; }
    public IEnumerable<string> Genres { get; set; } = Enumerable.Empty<string>();

}
