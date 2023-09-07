using Dapper;
using Movies.Application.database;

namespace Movies.Application.Repositories; 

public class RatingRepository: IRatingRepository{
    
    private readonly IDbConnectionFactory _connectionFactory;
    
    public RatingRepository(IDbConnectionFactory connectionFactory) {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> RateMovieAsync(Guid movieId, Guid userId, int rating, CancellationToken cancellationToken = default) {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        var result = await connection.ExecuteAsync(new CommandDefinition("""
            insert into ratings (movie_id, user_id, rating)
            values (@MovieId, @UserId, @Rating)
            on conflict (movie_id, user_id) do update
            set rating = @Rating;
        """, new { MovieId = movieId, UserId = userId, Rating = rating }, cancellationToken: cancellationToken));
        
        return result > 0;
    }

    public async Task<float?> GetRatingAsync(Guid movieId, CancellationToken cancellationToken = default) {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<float?>(new CommandDefinition("""
            select round(avg(rating), 1) from ratings r 
            where movie_id = @MovieId;
        """, new { MovieId = movieId }, cancellationToken: cancellationToken));
    }

    public async Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId, CancellationToken cancellationToken = default) {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<(float?, int?)>(new CommandDefinition("""
            select round(avg(rating), 1), 
                     (select rating
                      from ratings
                      where movie_id = @MovieId and user_id = @UserId) limit 1)
            from ratings 
            where movie_id = @MovieId;
        """, new { MovieId = movieId, UserId = userId }, cancellationToken: cancellationToken));
    }
}