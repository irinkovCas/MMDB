using Movies.Application.Models;

namespace Movies.Application.Services;

public interface IMovieService {

    public Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token = default);
}
