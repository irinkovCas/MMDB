namespace Movies.Api;

public class ApiEndpoints {
    private const string ApiBase = "/api";

    public static class Authentication {
        private const string Base = ApiBase + "/authentication";

        public const string Token = Base + "/token";
    }

    public static class Movies {

        private const string Base = ApiBase + "/movies";

        public const string Get = Base + "/{idOrSlug}";
        public const string GetAll = Base;
        public const string Create = Base;
        public const string Rate = Base + "/rate/{idOrSlug}";
        public const string Delete = Base + "/{id}";
        public const string Update = Base + "/{id}";

    }

}
