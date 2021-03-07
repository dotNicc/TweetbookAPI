using System.Collections.Generic;

namespace TweetBook.Contract.V1.Requests
{
    public class CreatePostRequest
    {
        public string Name { get; set; }
        public List<string> Tags { get; set;}
    }
}