using Movies.Application;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api; 

public static class Mapping {
    public static Movie MapToMovie(this CreateMovieRequest request) => new() {
        Id = Guid.NewGuid(),
        Title = request.Title,
        YearOfRelease = request.YearOfRelease,
        Genres = request.Genres.ToList()
    };
    
    public static MovieResponse MapToResponse(this Movie movie) => new() {
        Id = movie.Id,
        Title = movie.Title,
        Slug = movie.Slug,
        YearOfRelease = movie.YearOfRelease,
        Genres = movie.Genres
    };
    
    public static Movie MapToMovie(this UpdateMovieRequest movie, Guid id) => new() {
        Id = id,
        Title = movie.Title,
        YearOfRelease = movie.YearOfRelease,
        Genres = movie.Genres.ToList()
    };
}