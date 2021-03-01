using System.Linq;
using Microsoft.AspNetCore.Http;

namespace TweetBook.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetUserIdFromClaim(this HttpContext httpContext)
        {
            return httpContext.User.Claims.Single(x => x.Type == "id").Value;
        }
    }
}