using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers; 

[ApiController]
public class RatingController : ControllerBase {
    private readonly IRatingService _ratingService;

    public RatingController(IRatingService ratingService) {
        _ratingService = ratingService;
    }

    [Authorize]
    [HttpPut(ApiEndpoints.Movies.Rate)]
    public async Task<IActionResult> RateMovie([FromRoute] Guid id, [FromBody] RateMovieRequest request, CancellationToken cancellationToken = default) {
        Guid? userId = HttpContext.GetUserId();
        var result = await _ratingService.RateMovieAsync(id, userId!.Value, request.Rating, cancellationToken);
        return result ? Ok() : NotFound();
    }
    
    [Authorize]
    [HttpDelete(ApiEndpoints.Movies.DeleteRating)]
    public async Task<IActionResult> DeleteRating([FromRoute] Guid id, CancellationToken cancellationToken = default) {
        Guid? userId = HttpContext.GetUserId();
        var result = await _ratingService.DeleteRatingAsync(id, userId!.Value, cancellationToken);
        return result ? Ok() : NotFound();
    }
    
    [Authorize]
    [HttpGet(ApiEndpoints.Movies.GetUserRatings)]
    public async Task<IActionResult> GetUserRatings(CancellationToken cancellationToken = default) {
        Guid? userId = HttpContext.GetUserId();
        var ratings = await _ratingService.GetUserRatingsAsync(userId!.Value, cancellationToken);
        var ratingsResponse = ratings.MapToResponse();
        return Ok(ratingsResponse);
    }
}