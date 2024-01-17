using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers;

[ApiController]
public class MoviesController : ControllerBase {

    private readonly IMovieService movieService;

    public MoviesController(IMovieService movieService) {
        this.movieService = movieService;
    }

    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAllMovies([FromQuery] GetAllMoviesRequest request, CancellationToken cancellationToken) {
        var movies = await movieService.GetAllAsync(cancellationToken);

        var response = new GetAllMoviesResponse() {
            Movies = movies.Select(m => m.ToDto())
        };
    return Ok(movies);

}

}
