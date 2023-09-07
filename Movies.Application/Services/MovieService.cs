﻿using FluentValidation;
using Movies.Application.Repositories;
using Movies.Application.Validators;

namespace Movies.Application.Services; 

public class MovieService : IMovieService {
    private readonly IMovieRepository _movieRepository;
    private readonly IRatingRepository _ratingRepository;
    private readonly IValidator<Movie> _movieValidator;

    public MovieService(IMovieRepository movieRepository, IValidator<Movie> movieValidator, IRatingRepository ratingRepository) {
        _movieRepository = movieRepository;
        _movieValidator = movieValidator;
        _ratingRepository = ratingRepository;
    }
    
    public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default) {
        await _movieValidator.ValidateAndThrowAsync(movie, token);
        return await _movieRepository.CreateAsync(movie, token);
    }
    
    public async Task<bool> CreateBatchAsync(List<Movie> movies, CancellationToken token = default) {
        foreach (var movie in movies) {
            await _movieValidator.ValidateAndThrowAsync(movie, token);
        }
        return await _movieRepository.CreateBatchAsync(movies, token);
    }

    public Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default) {
        return _movieRepository.GetByIdAsync(id, userId, token);
    }

    public Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default) {
        return _movieRepository.GetBySlugAsync(slug, userId, token);
    }

    public Task<IEnumerable<Movie>> GetAllAsync(Guid? userId = default, CancellationToken token = default) {
        return _movieRepository.GetAllAsync(userId, token);
    }

    public async Task<Movie?> UpdateAsync(Movie movie, Guid? userId = default, CancellationToken token = default) {
        await _movieValidator.ValidateAndThrowAsync(movie, token);
        var movieExists = await _movieRepository.ExistsAsync(movie.Id, token);
        if (!movieExists) {
            return null;
        }
        
        await _movieRepository.UpdateAsync(movie, token);
        
        if (!userId.HasValue) {
            var rating = await _ratingRepository.GetRatingAsync(movie.Id, token);
            movie.Rating = rating;
        } else {
            var rating = await _ratingRepository.GetRatingAsync(movie.Id, userId.Value, token);
            movie.Rating = rating.Rating;
            movie.UserRating = rating.UserRating;
        }
        
        return movie;
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken token = default) {
        return _movieRepository.DeleteAsync(id, token);
    }
}