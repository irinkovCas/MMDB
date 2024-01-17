using Movies.Application.Models;
using Movies.Application.Repositorires;

namespace Movies.Application.Services;

public class MovieService : IMovieService {

    private readonly IMovieRepository movieRepository;

    public MovieService(IMovieRepository movieRepository) {
        this.movieRepository = movieRepository;
    }

    public Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token = default) {
        return movieRepository.GetAllAsync(token);
    }

}
