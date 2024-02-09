using Movies.Application.Models;
using Movies.Contracts.Entities;
using Movies.Contracts.Requests;

namespace Movies.Api.Mapping;

public static class Mapping {

    public static Movie MapToMovie(this CreateMovieRequest request) => new() {
        Id = Guid.NewGuid(),
        Title = request.Title,
        YearOfRelease = request.YearOfRelease,
        Genres = request.Genres.Select(x => new Genre { Name = x }).ToList(),
    };

    public static MovieDto MapToDto(this Movie movie) => new() {
        Id = movie.Id,
        Title = movie.Title,
        YearOfRelease = movie.YearOfRelease,
        Genres = movie.Genres.Select(x => x.Name).ToList(),
        Rating = movie.Rating
    };

    public static Movie MapToMovie(this UpdateMovieRequest movie, Guid id) => new() {
        Id = id,
        Title = movie.Title,
        YearOfRelease = movie.YearOfRelease,
        Genres = movie.Genres.Select(x => new Genre { Name = x }).ToList()
    };

}
