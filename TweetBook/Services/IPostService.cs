using System;
using System.Collections.Generic;
using TweetBook.Domain;

namespace TweetBook.Services
{
    public interface IPostService
    {
        List<Post> GetPosts();
        Post GetPostById(Guid postId);
        Post Create(Guid postId, string userId);
        bool UpdatePost(Post postToUpdate);
        bool DeletePost(Guid postId);
        bool UserOwnsPost(Guid postId, string userId);
    }
}