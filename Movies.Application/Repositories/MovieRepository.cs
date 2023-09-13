using Dapper;
using Movies.Application.database;

namespace Movies.Application.Repositories; 

public class MovieRepository : IMovieRepository{
    
    private readonly IDbConnectionFactory _connectionFactory;
    
    public MovieRepository(IDbConnectionFactory connectionFactory) {
        _connectionFactory = connectionFactory;
    }
    
    public async Task<bool> CreateAsync(Movie movie, CancellationToken cancellationToken = default) {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        var result = await connection.ExecuteAsync(new CommandDefinition("""
            insert into movies (id, slug, title, yearofrelease) 
            values (@Id, @Slug, @Title, @YearOfRelease);
        """, movie, transaction: transaction, cancellationToken: cancellationToken));
        
        if (result == 0) {
            return false;
        }
        
        for (int i = 0; i < movie.Genres.Count; i++) {
            var genre = movie.Genres[i];
            await connection.ExecuteAsync(new CommandDefinition("""
                insert into genres (movie_id, name) 
                values (@MovieId, @Name);
            """, new { MovieId = movie.Id, Name = genre }, transaction: transaction, cancellationToken: cancellationToken));
        }
        
        transaction.Commit();
        
        return true;
    }
    
    class MovieGenre {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
    
    public async Task<bool> CreateBatchAsync(List<Movie> movies, CancellationToken cancellationToken = default) {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        // log how many movies are being created
        
        var result = await connection.ExecuteAsync(new CommandDefinition("""
            insert into movies (id, slug, title, yearofrelease) 
            values (@Id, @Slug, @Title, @YearOfRelease);
        """, movies, transaction: transaction, cancellationToken: cancellationToken));
        
        
        if (result == 0) {
            return false;
        }
        
        IEnumerable<MovieGenre> genres = movies.SelectMany(movie => movie.Genres.Select(genre => new MovieGenre { Id = movie.Id, Name = genre }));
        
        await connection.ExecuteAsync(new CommandDefinition("""
            insert into genres (movie_id, name) 
            values (@Id, @Name);
        """, genres, transaction: transaction, cancellationToken: cancellationToken));
        
        // Exception – An error occurred while trying to commit the transaction.
        // InvalidOperationException – The transaction has already been committed or rolled back. -or- The connection is broken.
        transaction.Commit();
        return true;
    }

    public async Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken cancellationToken = default) {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition("""
            select m.*, round(avg(r.rating), 1) as rating, myr.rating as userrating
            from movies m
            left join ratings r on m.id = r.movie_id
            left join ratings myr on m.id = myr.movie_id and myr.user_id = @userId
            where id = @id
            group by id, userrating
        """, new { id, userId }, cancellationToken: cancellationToken));
        
        if (movie == null) {
            return null;
        }
        
        var genres = await connection.QueryAsync<string>(new CommandDefinition("""
            select name from genres where movie_id = @id;
        """, new { Id = id }, cancellationToken: cancellationToken));

        foreach (var genre in genres) {
            movie.Genres.Add(genre);
        }

        return movie;
    }
    
    public async Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken cancellationToken = default) {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition("""
            select m.*, round(avg(r.rating), 1) as rating, myr.rating as userrating
            from movies m
            left join ratings r on m.id = r.movie_id
            left join ratings myr on m.id = myr.movie_id and myr.user_id = @userId
            where slug = @slug
            group by id, userrating
        """, new { slug, userId }, cancellationToken: cancellationToken));
        
        if (movie == null) {
            return null;
        }
        
        var genres = await connection.QueryAsync<string>(new CommandDefinition("""
            select name from genres where movie_id = @id;
        """, new { id = movie.Id }, cancellationToken: cancellationToken));

        foreach (var genre in genres) {
            movie.Genres.Add(genre);
        }

        return movie;
    }
    public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken cancellationToken = default) {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        
        var orderClause = string.Empty;
        if (options.SortField != null) {
            orderClause = $"order by {options.SortField} {(options.SortOrder == SortOrder.Descending ? "desc" : "asc")}";
        }
        
        var result = await connection.QueryAsync(new CommandDefinition($"""
            select m.*, 
                   string_agg(distinct g.name, ',') as genres,
                   round(avg(r.rating), 1) as rating,
                   myr.rating as userrating
            from movies m 
                left join genres g on m.id = g.movie_id
                left join ratings r on m.id = r.movie_id
                left join ratings myr on m.id = myr.movie_id and myr.user_id = @userId
            where (@title is null or m.title like ('%' || @title || '%'))
              and (@yearOfRelease is null or m.yearofrelease = @yearOfRelease)
            group by id, userrating {orderClause}
            limit @pageSize 
            offset @offset;
        """, new {
            userId = options.UserId, 
            yearOfRelease = options.Year,
            title = options.Title,
            pageSize = options.PageSize,
            offset = (options.Page - 1) * options.PageSize
        } , cancellationToken: cancellationToken));

        return result.Select(x => new Movie {
            Id = x.id,
            Title = x.title,
            YearOfRelease = x.yearofrelease,
            UserRating = (int?)x.userrating,
            Rating = (float?)x.rating,
            Genres = Enumerable.ToList(x.genres.Split(','))
        });
    }
    
    public async Task<bool> UpdateAsync(Movie movie, CancellationToken cancellationToken = default) { 
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
            delete from genres where movie_id = @id;
        """, new { id = movie.Id }, transaction: transaction, cancellationToken: cancellationToken));
        
        foreach (var genre in movie.Genres) {
            await connection.ExecuteAsync(new CommandDefinition("""
                insert into genres (movie_id, name) 
                values (@MovieId, @Name);
            """, new { MovieId = movie.Id, Name = genre }, transaction: transaction, cancellationToken: cancellationToken));
        }
        
        var result = await connection.ExecuteAsync(new CommandDefinition("""
            update movies set title = @Title, yearofrelease = @YearOfRelease where id = @Id;
        """, movie, transaction: transaction, cancellationToken: cancellationToken));
        
        transaction.Commit();
        return result > 0;
    }
    
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default) {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();
        
        await connection.ExecuteAsync(new CommandDefinition("""
            delete from genres where movie_id = @id;
        """, new { id }, transaction: transaction, cancellationToken: cancellationToken));
        
        var result = await connection.ExecuteAsync(new CommandDefinition("""
            delete from movies where id = @id;
        """, new { id }, transaction: transaction, cancellationToken: cancellationToken));
        
        transaction.Commit();
        return result > 0;
    }
    
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition("""
            select exists(select 1 from movies where id = @id);
        """, new { id }, cancellationToken: cancellationToken));
    }

    public async Task<int> GetCountAsync(string? title, int? yearOfRelease, CancellationToken cancellationToken = default) {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<int>(new CommandDefinition("""
            select count(*) from movies where (@title is null or title like ('%' || @title || '%')) and (@yearOfRelease is null or yearofrelease = @yearOfRelease);
        """, new { title, yearOfRelease }, cancellationToken: cancellationToken));
        
    }
}