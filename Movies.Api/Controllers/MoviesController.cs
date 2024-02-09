using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Models;
using Movies.Application.Services;
using Movies.Contracts.Entities;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers;

[ApiController]
public class MoviesController : ControllerBase {

    private readonly IMovieService movieService;

    public MoviesController(IMovieService movieService) {
        this.movieService = movieService;
    }

    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPost(ApiEndpoints.Movies.Create)]
    [ProducesResponseType(typeof(MovieDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken token) {
        var movie = request.MapToMovie();
        await movieService.CreateMovieAsync(movie, token);
        MovieDto movieResponse = movie.MapToDto();

        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movieResponse);
    }

    [HttpGet(ApiEndpoints.Movies.Get)]
    //[ResponseCache(Duration = 30, VaryByHeader = "Accept, Accept-Encoding", Location = ResponseCacheLocation.Any)]
    [ProducesResponseType(typeof(MovieDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken token) {
        var userId = HttpContext.GetUserId();

        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await movieService.GetByIdAsync(id, userId, token)
            : await movieService.GetBySlugAsync(idOrSlug, userId, token);

        if (movie is null) {
            return NotFound();
        }

        var response = movie.MapToDto();

        return Ok(response);
    }

    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAllMovies([FromQuery] GetAllMoviesRequest request, CancellationToken cancellationToken) {
        var movies = await movieService.GetMoviesAllAsync(request, cancellationToken);

        var response = new GetAllMoviesResponse() {
            Movies = movies.Select(m => m.MapToDto())
        };

        return Ok(response);
    }

    [HttpPost(ApiEndpoints.Movies.Rate)]
    public async Task<IActionResult> RateMovie([FromRoute] Guid idOrSlug, [FromBody] RateMovieRequest request, CancellationToken cancellationToken) {
        var result = await movieService.RateMovieAsync(idOrSlug, request.Rating, cancellationToken);

        if (!result) {
            return NotFound();
        }

        return Ok();
    }

    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPut(ApiEndpoints.Movies.Update)]
    [ProducesResponseType(typeof(MovieDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateMovie([FromRoute] Guid id, [FromBody] UpdateMovieRequest request, CancellationToken cancellationToken) {
        Movie movie = request.MapToMovie(id);
        Guid? userId = HttpContext.GetUserId();

        Movie? updatedMovie = await movieService.UpdateMovieAsync(movie, cancellationToken);

        if (updatedMovie is null) {
            return NotFound();
        }

        MovieDto response = updatedMovie.MapToDto();
        return Ok(response);
    }

    [Authorize(AuthConstants.AdminUserPolicyName)]
    [HttpDelete(ApiEndpoints.Movies.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMovie([FromRoute] Guid id, CancellationToken cancellationToken) {
        var result = await movieService.DeleteMovieAsync(id, cancellationToken);

        if (!result) {
            return NotFound();
        }

        return Ok();
    }

}
