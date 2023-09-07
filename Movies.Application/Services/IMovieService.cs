namespace Movies.Application.Services; 

public interface IMovieService {
    Task<bool> CreateAsync(Movie movie, CancellationToken token = default);
    Task<bool> CreateBatchAsync(List<Movie> movies, CancellationToken token = default);
    Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default);
    Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default);
    Task<IEnumerable<Movie>> GetAllAsync(Guid? userId = default, CancellationToken token = default);
    Task<Movie?> UpdateAsync(Movie movie, Guid? userId = default, CancellationToken token = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken token = default);
}