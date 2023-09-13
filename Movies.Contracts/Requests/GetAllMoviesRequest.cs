namespace Movies.Contracts.Requests; 

public class GetAllMoviesRequest : PagedRequest {
    public string? Title { get; set; }
    public int? Year { get; set; }
    public string? SortBy { get; set; }
}