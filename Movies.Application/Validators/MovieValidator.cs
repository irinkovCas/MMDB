﻿using FluentValidation;
using Movies.Application.Repositories;
using Movies.Application.Services;

namespace Movies.Application.Validators; 

public class MovieValidator : AbstractValidator<Movie> {
    private readonly IMovieRepository _movieRepository;
    
    public MovieValidator(IMovieRepository movieRepository) {
        _movieRepository = movieRepository;
        
        RuleFor(movie => movie.Title).NotEmpty();  //.MaximumLength(100);
        RuleFor(movie => movie.Genres).NotEmpty();
        RuleFor(movie => movie.Id).NotEmpty();
        RuleFor(movie => movie.YearOfRelease).LessThanOrEqualTo(DateTime.UtcNow.Year);
        RuleFor(movie => movie.Slug).MustAsync(ValidateSlug).WithMessage("This movie already exists in the system");
    }
    
    private async Task<bool> ValidateSlug(Movie movie, string slug, CancellationToken cancellationToken) {
        var existingMovie = await _movieRepository.GetBySlugAsync(slug);
        return existingMovie == null || existingMovie.Id == movie.Id;
    }
}