using Movies.Application.Models;

namespace Movies.Application.Repositorires {

    public interface IMovieRepository {

        Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token = default);

    }

}
