namespace Movies.Api; 

public static class ApiEndpoints {
    private const string ApiBase = "/api";

    public static class Authentication {
        private const string Base = ApiBase + "/authentication";
        
        public const string Token = Base + "/token";
    }

    public static class Movies {
        private const string Base = ApiBase + "/movies";
        
        public const string Create = Base;
        public const string CreateBatch = $"{Base}/batch";
        public const string Get = $"{Base}/{{idOrSlug}}";
        public const string GetAll = Base;
        public const string Update = $"{Base}/{{id:guid}}"; // this will only accept a guid
        public const string Delete = $"{Base}/{{id:guid}}";
        public const string DeleteAll = $"{Base}/all";
        
        public const string Rate = $"{Base}/{{id:guid}}/rating";
    }
}