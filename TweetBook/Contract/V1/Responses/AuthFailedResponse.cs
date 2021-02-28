using System.Collections.Generic;

namespace TweetBook.Contract.V1.Responses
{
    public class AuthFailedResponse
    {
        public IEnumerable<string> Errors { get; set; }
    }
}