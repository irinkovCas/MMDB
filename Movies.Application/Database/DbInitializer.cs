using Dapper;

namespace Movies.Application.database; 

public class DbInitializer {
    private readonly IDbConnectionFactory _connectionFactory;
    
    public DbInitializer(IDbConnectionFactory connectionFactory) {
        _connectionFactory = connectionFactory;
    }
    
    public async Task InitializeAsync() {
        var connection = await _connectionFactory.CreateConnectionAsync();
        await connection.ExecuteAsync("""    
            create table if not exists movies (
                id UUID primary key,
                slug TEXT not null,
                title TEXT not null,
                yearofrelease integer not null
           );
        """);
        // create a index on the slug column
        await connection.ExecuteAsync("""
            create unique index concurrently if not exists movies_slug_idx 
            on movies 
            using btree(slug);
        """);
        
        await connection.ExecuteAsync("""    
            create table if not exists genres (
                movie_id UUID references movies(id),
                name TEXT not null
           );
        """);
        
        var res = await connection.ExecuteAsync("""    
            create table if not exists ratings (
                user_id UUID,
                movie_id UUID references movies(id),
                rating integer not null,
                primary key (user_id, movie_id)
           );
        """);
        
        Console.WriteLine(res);
    }
}