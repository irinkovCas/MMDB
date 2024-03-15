namespace Movies.Contracts.Responses;

using DTOs;

public class GetAllMoviesResponse
{

    public IEnumerable<MovieDto> Movies { get; set; } = Enumerable.Empty<MovieDto>();

}
