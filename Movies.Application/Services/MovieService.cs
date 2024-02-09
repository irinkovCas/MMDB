using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositorires;
using Movies.Contracts.Requests;

namespace Movies.Application.Services;

public class MovieService : IMovieService {

    private readonly IMovieRepository movieRepository;
    private readonly IValidator<Movie> movieValidator;
    private readonly IValidator<GetAllMoviesRequest> optionsValidator;

    public MovieService(IMovieRepository movieRepository, IValidator<Movie> movieValidator, IValidator<GetAllMoviesRequest> optionsValidator) {
        this.movieRepository = movieRepository;
        this.movieValidator = movieValidator;
        this.optionsValidator = optionsValidator;
    }

    public async Task<IEnumerable<Movie>> GetMoviesAllAsync(GetAllMoviesRequest options, CancellationToken token = default) {
        await optionsValidator.ValidateAndThrowAsync(options, token);
        return await movieRepository.GetAllAsync(token);
    }

    public Task<float?> GetAverageRatingAsync(Guid movieId, CancellationToken token = default) {
        return movieRepository.GetAverageRatingAsync(movieId, token);
    }

    public async Task<bool> CreateMovieAsync(Movie movie, CancellationToken cancellationToken) {
        await movieValidator.ValidateAndThrowAsync(movie, cancellationToken);
        return await movieRepository.CreateMovieAsync(movie, cancellationToken);
    }

    public Task<bool> RateMovieAsync(Guid id, int requestRating, CancellationToken cancellationToken) {
        return movieRepository.RateMovieAsync(id, requestRating, cancellationToken);
    }

    public Task<bool> DeleteMovieAsync(Guid id, CancellationToken cancellationToken) {
        return movieRepository.DeleteMovieAsync(id, cancellationToken);
    }

    public Task<Movie?> UpdateMovieAsync(Movie movie, CancellationToken cancellationToken) {
        return movieRepository.UpdateMovieAsync(movie, cancellationToken);
    }

    public Task<Movie> GetByIdAsync(Guid movieId, Guid? userId, CancellationToken token) {
        return movieRepository.GetByIdAsync(movieId, userId, token);
    }

    public Task<Movie> GetBySlugAsync(string slug, Guid? userId, CancellationToken token) {
        return movieRepository.GetBySlugAsync(slug, userId, token);
    }

}
