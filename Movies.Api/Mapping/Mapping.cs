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
    
    public static MovieResponse MapToResponse(this Movie movie, int? page = null, int? pageSize = null, int? total = null) => new() {
        Id = movie.Id,
        Title = movie.Title,
        Slug = movie.Slug,
        YearOfRelease = movie.YearOfRelease,
        UserRating = movie.UserRating,
        Rating = movie.Rating,
        Genres = movie.Genres,
        // Page = page,
    };
    
    public static Movie MapToMovie(this UpdateMovieRequest movie, Guid id) => new() {
        Id = id,
        Title = movie.Title,
        YearOfRelease = movie.YearOfRelease,
        Genres = movie.Genres.ToList()
    };

    public static IEnumerable<MovieRatingResponse> MapToResponse(this IEnumerable<MovieRating> ratings) =>
        ratings.Select(x => new MovieRatingResponse { MovieId = x.MovieId, Slug = x.Slug, Rating = x.Rating });
    
    public static GetAllMoviesOptions MapToOption(this GetAllMoviesRequest request) => new() {
        Title = request.Title,
        Year = request.Year,
        SortField = request.SortBy?.Trim('+', '-'),
        SortOrder = request.SortBy == null ? SortOrder.Unsorted : 
            request.SortBy.StartsWith('-') ? SortOrder.Descending : SortOrder.Ascending,
        Page = request.Page,
        PageSize = request.PageSize
    };
    
    public static GetAllMoviesOptions WithUserId(this GetAllMoviesOptions options, Guid? userId) {
        options.UserId = userId;
        return options;
    } 
}