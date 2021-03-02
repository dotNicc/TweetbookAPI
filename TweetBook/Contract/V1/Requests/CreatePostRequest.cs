using System;

namespace TweetBook.Contract.V1.Requests
{
    public class CreatePostRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}