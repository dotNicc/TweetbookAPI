namespace TweetBook.Contract.V2
{
    public static class ApiRoutes
    {
        private const string Root = "api";
        private const string Version = "v2";
        private const string Base = Root + "/" + Version;

        public static class Tags
        {
            public const string GetAll = Base + "/tags";
        }
    }
}