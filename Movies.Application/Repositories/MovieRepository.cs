namespace Movies.Application.Repositories;

using Microsoft.EntityFrameworkCore;
using database;
using Entities;
using Models;

public class MovieRepository : IMovieRepository
{

    private readonly MovieDbContext movieDbContext;

    public MovieRepository(MovieDbContext movieDbContext)
    {
        this.movieDbContext = movieDbContext;
    }

    public async Task<ICollection<Movie>> GetAllAsync(CancellationToken token = default)
    {
        var moviesWithRatings = await this.movieDbContext.Movies
            .Include(m => m.Genres)
            .Select(m => new
            {
                Movie = m, AverageRating = m.Ratings.Any() ? m.Ratings.Average(r => r.Score) : (float?)null
            })
            .ToListAsync(token);

        var movies = moviesWithRatings.Select(mr =>
            {
                var movie = mr.Movie;
                movie.Rating = mr.AverageRating;

                return movie;
            })
            .ToList();

        return movies;
    }

    public async Task<float?> GetAverageRatingAsync(Guid movieId, CancellationToken token = default)
    {
        float? averageRating = null;

        try
        {
            averageRating = await this.movieDbContext.Ratings
                .Where(r => r.MovieId == movieId)
                .Select(r => r.Score)
                .AverageAsync(token);
        }
        catch (InvalidOperationException)
        {
        }

        return averageRating;
    }

    // v0
    // Will this work
    // public async Task<float?> GetAverageRatingAsync(Guid movieId, CancellationToken token = default) {
    //     Movie? movie = movieDbContext.Movies.Find(movieId);
    //
    //     if (movie == null) {
    //         return null;
    //     }
    //
    //     if (movie.Ratings == null || movie.Ratings.Count == 0) {
    //         return null;
    //     }
    //
    //     float averageRating = movie.Ratings
    //         .Select(r => r.Score)
    //         .Average();
    //
    //     return averageRating;
    // }

    // v1
    // public async Task<float?> GetAverageRatingAsync(Guid movieId, CancellationToken token = default) {
    //     // Directly calculate the average rating from the related Ratings of the Movie
    //     var averageRating = await movieDbContext.Movies
    //         .Where(m => m.Id == movieId)
    //         .SelectMany(m => m.Ratings.Select(r => (float?)r.Score))
    //         .AverageAsync(token);
    //     // mayeb need to try catch the AverageAsync
    //     return averageRating;
    // }

    // v2
    // public async Task<float?> GetAverageRatingAsync(Guid movieId, CancellationToken token = default) {
    //     // Attempt to calculate the average rating directly
    //     float? averageRating = await movieDbContext.Ratings
    //         .Where(r => r.MovieId == movieId)
    //         .Select(r => (float?)r.Score)
    //         .AverageAsync(token);
    //
    //     return averageRating;
    // }
    public async Task<bool> CreateMovieAsync(Movie movie, CancellationToken cancellationToken)
    {
        movie.Slug = movie.GenerateSlug();

        var genreNames = movie.Genres.Select(g => g.Name).ToList();

        var existingGenres = await this.movieDbContext.Genres
            .Where(g => genreNames.Contains(g.Name))
            .ToListAsync(cancellationToken);

        movie.Genres.Clear();

        foreach (var genreName in genreNames)
        {
            var genre = existingGenres.FirstOrDefault(g => g.Name == genreName) ??
                this.movieDbContext.Genres.Local.FirstOrDefault(g => g.Name == genreName) ??
                new Genre
                {
                    Name = genreName
                };

            movie.Genres.Add(genre);
        }

        this.movieDbContext.Movies.Add(movie);

        var saveResult = await this.movieDbContext.SaveChangesAsync(cancellationToken);

        return saveResult > 0;
    }

    public Task<bool> RateMovieAsync(Guid id, int requestRating, CancellationToken cancellationToken)
    {
        this.movieDbContext.Ratings.Add(new Rating()
        {
            MovieId = id, Score = requestRating
        });

        return this.movieDbContext
            .SaveChangesAsync(cancellationToken)
            .ContinueWith(t => t.Result > 0, cancellationToken);
    }

    public Task<bool> DeleteMovieAsync(Guid idOrSlug, CancellationToken cancellationToken)
    {
        // System.InvalidOperationException: The instance of entity type 'Movie' cannot be tracked because another instance
        // with the same key value for {'Id'} is already being tracked. When attaching existing entities,
        // ensure that only one entity instance with a given key value is attached.
        // Consider using 'DbContextOptionsBuilder.EnableSensitiveDataLogging' to see the conflicting key values.
        //
        // movieDbContext.Movies.Remove(new Movie() {
        //     Id = idOrSlug
        // });

        Movie? movie = this.movieDbContext.Movies.Find(idOrSlug);

        if (movie == null)
        {
            return Task.FromResult(false);
        }

        this.movieDbContext.Movies.Remove(movie);

        return this.movieDbContext
            .SaveChangesAsync(cancellationToken)
            .ContinueWith(t => t.Result > 0, cancellationToken);
    }

    public async Task<Movie?> UpdateMovieAsync(Movie movie, CancellationToken cancellationToken)
    {
        var existingMovie = await this.movieDbContext.Movies
            .Include(m => m.Genres)
            .FirstOrDefaultAsync(m => m.Id == movie.Id, cancellationToken);

        if (existingMovie == null)
        {
            return null;
        }

        if (existingMovie.Genres != null) existingMovie.Genres.Clear();

        var genreNames = movie.Genres.Select(g => g.Name).ToList();

        var newGenres = await this.movieDbContext.Genres
            .Where(g => genreNames.Contains(g.Name))
            .ToListAsync(cancellationToken);

        foreach (var genre in newGenres)
        {
            existingMovie.Genres.Add(genre);
        }

        var newGenreNames = newGenres.Select(g => g.Name);
        var missingGenres = movie.Genres.Where(g => !newGenreNames.Contains(g.Name)).ToList();

        foreach (var missingGenre in missingGenres)
        {
            existingMovie.Genres.Add(missingGenre);
        }

        existingMovie.Title = movie.Title;
        existingMovie.YearOfRelease = movie.YearOfRelease;

        var result = await this.movieDbContext.SaveChangesAsync(cancellationToken);

        return result > 0 ? existingMovie : null;
    }

    public Task<Movie?> GetBySlugAsync(string slug, Guid? userId, CancellationToken token)
    {
        return this.movieDbContext.Movies
            .Where(m => m.Slug == slug)
            .FirstOrDefaultAsync(token);
    }

    public Task<Movie?> GetByIdAsync(Guid id, Guid? userId, CancellationToken token)
    {
        return this.movieDbContext.Movies
            .Where(m => m.Id == id)
            .FirstOrDefaultAsync(token);
    }

    public async Task<ICollection<Rating>> GetRatingsForUserAsync(Guid userId, CancellationToken token)
    {
        List<Rating> ratings = await this.movieDbContext.Ratings
            .Where(r => r.UserId == userId)
            .ToListAsync(token);

        return ratings;
    }

}
