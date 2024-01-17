using Movies.Application.database;
using Movies.Application.Models;

namespace Movies.Application.Repositorires {

    public class MovieRepository : IMovieRepository {

        private readonly IDbConnectionFactory dbConnectionFactory;

        public MovieRepository(IDbConnectionFactory dbConnectionFactory) {
        }

        public Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token = default) {
            return Task.FromResult(Enumerable.Empty<Movie>());
        }

    }

}
