using System.Data;
using Npgsql;

namespace Movies.Application.database; 

public interface IDbConnectionFactory {
    Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}