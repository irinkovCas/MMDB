﻿namespace Movies.Application.Services; 

public interface IRatingService {
    Task<bool> RateMovieAsync(Guid movieId, Guid userId, int rating, CancellationToken cancellationToken = default);
    Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MovieRating>> GetUserRatingsAsync(Guid userId, CancellationToken cancellationToken = default);
}