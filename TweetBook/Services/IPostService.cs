using System;
using System.Collections.Generic;
using LanguageExt;
using TweetBook.Domain;

namespace TweetBook.Services
{
    public interface IPostService
    {
        List<Post> GetPosts();
        Option<Post> GetPostById(Guid postId);
        Post Create(string name, List<string> tags, string userId);
        bool UpdatePost(Post postToUpdate);
        bool DeletePost(Guid postId);
        bool UserOwnsPost(Guid postId, string userId);
        List<Tag> GetAllTags();
        bool DeleteTag(string tagName);
    }
}