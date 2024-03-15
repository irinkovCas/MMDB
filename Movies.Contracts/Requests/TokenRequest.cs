namespace Movies.Contracts.Requests;

public class TokenRequest
{

    public required Guid UserId { get; init; }
    public required string Email { get; init; }
    public required Dictionary<string, object> CustomClaims { get; init; } = new Dictionary<string, object>();

}
