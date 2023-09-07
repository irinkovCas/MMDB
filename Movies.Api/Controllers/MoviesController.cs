using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Application;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers;

[ApiController]
public class MoviesController : ControllerBase {
    private readonly IMovieService _movieService;
    
    public MoviesController(IMovieService movieService) {
        _movieService = movieService;
    }

    [Authorize("Admin")]
    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken cancellationToken) {
        Movie movie = request.MapToMovie();
        await _movieService.CreateAsync(movie, cancellationToken);
        return CreatedAtAction(nameof(GetMovie), new { idOrSlug = movie.Id }, movie.MapToResponse());
    }
    
    [Authorize("Admin")]
    [HttpPost(ApiEndpoints.Movies.CreateBatch)]
    public async Task<IActionResult> CreateBatch([FromBody] CreateMoviesRequest request, CancellationToken cancellationToken) {
        var movies = request.Movies.Select(movie => movie.MapToMovie()).ToList();
        await _movieService.CreateBatchAsync(movies, cancellationToken);
        // return CreatedAtAction(nameof(GetMovie), new { ids = movies.Select< string>( movie => movie.id) });
        return Created("", "");
    }

    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetMovies(CancellationToken cancellationToken) {
        var userId = HttpContext.GetUserId();

        var movies = await _movieService.GetAllAsync(userId, cancellationToken);
        var response = movies.Select(movie => movie.MapToResponse());
        return Ok(response);
    }
    
    [HttpGet(ApiEndpoints.Movies.Get)]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMovie([FromRoute] string idOrSlug, CancellationToken cancellationToken) {
        var userId = HttpContext.GetUserId();
        
        var movie = Guid.TryParse(idOrSlug, out var id) 
            ? await _movieService.GetByIdAsync(id, userId, cancellationToken) 
            : await _movieService.GetBySlugAsync(idOrSlug, userId, cancellationToken);
        if (movie is null) {
            return NotFound();
        }
        return Ok(movie.MapToResponse());
    }
    
    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> UpdateMovie([FromRoute] Guid id, [FromBody] UpdateMovieRequest request, CancellationToken cancellationToken) {
        var userId = HttpContext.GetUserId();

        var movie = request.MapToMovie(id);
        var updatedMovie = await _movieService.UpdateAsync(movie, userId, cancellationToken);
        if (updatedMovie is null) {
            return NotFound();
        }
        
        var response = await _movieService.GetByIdAsync(id, userId, cancellationToken);
        return Ok(response.MapToResponse());
    }
    
    [HttpDelete(ApiEndpoints.Movies.Delete)]
    public async Task<IActionResult> DeleteMovie([FromRoute] Guid id, CancellationToken cancellationToken) {
        var deleted = await _movieService.DeleteAsync(id, cancellationToken);
        if (!deleted) {
            return NotFound();
        }
        
        return NoContent();
    }
}