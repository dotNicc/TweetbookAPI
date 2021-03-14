using System;
using System.Collections.Generic;

namespace TweetBook.Domain
{
    public class Post
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public List<Tag> Tags { get; set; }
    }
}