using Movies.Application.Models;
using Movies.Contracts.Requests;

namespace Movies.Application.Services;

public interface IMovieService {

    Task<IEnumerable<Movie>> GetMoviesAllAsync(GetAllMoviesRequest options, CancellationToken token = default);
    Task<bool> CreateMovieAsync(Movie movie, CancellationToken cancellationToken);
    Task<bool> RateMovieAsync(Guid id, int requestRating, CancellationToken cancellationToken);
    Task<bool> DeleteMovieAsync(Guid idOrSlug, CancellationToken cancellationToken);
    Task<Movie?> UpdateMovieAsync(Movie movie, CancellationToken cancellationToken);
    Task<Movie?> GetByIdAsync(Guid result, Guid? userId, CancellationToken token);
    Task<Movie?> GetBySlugAsync(string idOrSlug, Guid? userId, CancellationToken token);

}
