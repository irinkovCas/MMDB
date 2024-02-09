using Movies.Application.Models;

namespace Movies.Application.Repositorires {

    public interface IMovieRepository {

        Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token = default);
        Task<float?> GetAverageRatingAsync(Guid movieId, CancellationToken token = default);

        Task<bool> CreateMovieAsync(Movie movie, CancellationToken cancellationToken);

        Task<bool> RateMovieAsync(Guid id, int requestRating, CancellationToken cancellationToken);

        Task<bool> DeleteMovieAsync(Guid idOrSlug, CancellationToken cancellationToken);

        Task<Movie?> UpdateMovieAsync(Movie movie, CancellationToken cancellationToken);

        Task<Movie?> GetBySlugAsync(string slug, Guid? userId, CancellationToken token);

        Task<Movie?> GetByIdAsync(Guid id, Guid? userId, CancellationToken token);

    }

}
