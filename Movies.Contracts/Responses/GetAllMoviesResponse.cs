using Movies.Contracts.Entities;

namespace Movies.Contracts.Responses;

public class GetAllMoviesResponse {

    public IEnumerable<MovieDto> Movies { get; set; } = Enumerable.Empty<MovieDto>();

}
