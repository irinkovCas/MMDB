namespace Movies.Contracts.Requests; 

public class CreateMoviesRequest { 
    public required List<CreateMovieRequest> Movies { get; init; } = new();
}