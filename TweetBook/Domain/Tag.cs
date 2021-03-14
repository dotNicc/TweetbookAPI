using System;

namespace TweetBook.Domain
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string TagType { get; set; }
    }
}