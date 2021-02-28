namespace TweetBook.Contract.V1
{
    public static class ApiRoutes
    {
        private const string Root = "api";
        private const string Version = "v1";
        private const string Base = Root + "/" + Version;

        public static class Posts
        {
            public const string GetAll = Base + "/posts";
            public const string Create = Base + "/posts";
            public const string Get = Base + "/posts/{postId}";
            public const string Update = Base + "/posts/{postId}";
            public const string Delete = Base + "/posts/{postId}";
        }

        // TODO breaks RESTful APIs rules, should be its own IdentityServer
        public static class Identity
        {
            public const string Login = Base + "/identity/login";
            public const string Register = Base + "/identity/register";
        }
    }
}