using Microsoft.EntityFrameworkCore;
using Movies.Application.database;
using Movies.Application.Models;

namespace Movies.Application.Repositorires {

    public class MovieRepository : IMovieRepository {

        private readonly MovieDbContext movieDbContext;

        public MovieRepository(MovieDbContext movieDbContext) {
            this.movieDbContext = movieDbContext;
        }

        public async Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token = default) {
            var movies = await movieDbContext.Movies
                .Include(m => m.Genres)
                .Include(m => m.Ratings)
                .ToListAsync(token);

            return movies;
        }

        public async Task<float?> GetAverageRatingAsync(Guid movieId, CancellationToken token = default) {
            // Check if the movie exists asynchronously
            bool movieExists = await movieDbContext.Movies
                .AnyAsync(m => m.Id == movieId, token);

            if (!movieExists) {
                return null;
            }

            // Directly calculate the average rating from the database
            float? averageRating = await movieDbContext.Ratings
                .Where(r => r.MovieId == movieId)
                .Select(r => (float?)r.Score)
                .AverageAsync(token);

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

        public Task<bool> CreateMovieAsync(Movie movie, CancellationToken cancellationToken) {
            movieDbContext.Movies.Add(movie);

            return movieDbContext
                .SaveChangesAsync(cancellationToken)
                .ContinueWith(t => t.Result > 0, cancellationToken);
        }

        public Task<bool> RateMovieAsync(Guid id, int requestRating, CancellationToken cancellationToken) {
            movieDbContext.Ratings.Add(new Rating() {
                MovieId = id,
                Score = requestRating
            });

            return movieDbContext
                .SaveChangesAsync(cancellationToken)
                .ContinueWith(t => t.Result > 0, cancellationToken);
        }

        public Task<bool> DeleteMovieAsync(Guid idOrSlug, CancellationToken cancellationToken) {
            movieDbContext.Movies.Remove(new Movie() {
                Id = idOrSlug
            });

            return movieDbContext
                .SaveChangesAsync(cancellationToken)
                .ContinueWith(t => t.Result > 0, cancellationToken);
        }

        public async Task<Movie?> UpdateMovieAsync(Movie movie, CancellationToken cancellationToken) {
            movieDbContext.Movies.Update(movie);

            int result = await movieDbContext.SaveChangesAsync(cancellationToken);

            if (result == 0) {
                return null;
            }

            return await movieDbContext.Movies.FindAsync(movie.Id);
        }

        public Task<Movie?> GetBySlugAsync(string slug, Guid? userId, CancellationToken token) {
            return movieDbContext.Movies
                .Where(m => m.Slug == slug)
                .FirstOrDefaultAsync(token);
        }

        public Task<Movie?> GetByIdAsync(Guid id, Guid? userId, CancellationToken token) {
            return movieDbContext.Movies
                .Where(m => m.Id == id)
                .FirstOrDefaultAsync(token);
        }

    }

}
