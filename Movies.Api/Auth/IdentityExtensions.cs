namespace Movies.Api.Auth;

public static class IdentityExtensions {
    public static Guid? GetUserId(this HttpContext context) {
        var userId = context.User.Claims.SingleOrDefault(x => x.Type == "userid")?.Value;
        
        if (Guid.TryParse(userId, out var id)) {
            return id;
        }
        
        return null;
    }
}
