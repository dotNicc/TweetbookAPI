using System;
using System.Linq;
using LanguageExt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TweetBook.Contract.V1;
using TweetBook.Contract.V1.Requests;
using TweetBook.Contract.V1.Responses;
using TweetBook.Domain;
using TweetBook.Extensions;
using TweetBook.Services;

namespace TweetBook.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostsController : Controller
    {
        private readonly IPostService postService;

        public PostsController(IPostService postService)
        {
            this.postService = postService;
        }

        [HttpGet(ApiRoutes.Posts.GetAll)]
        public IActionResult GetAll()
        {
            return Ok(this.postService
                .GetPosts()
                .Select(post => new PostResponse
                {
                    Id = post.Id,
                    Name = post.Name,
                    UserId = post.UserId,
                    Tags = post.Tags.Select(p => new TagResponse {Name = p.Name})
                }));
        }

        [HttpGet(ApiRoutes.Posts.Get)]
        public IActionResult Get([FromRoute] Guid postId)
        {
            Option<Post> postOption = this.postService.GetPostById(postId);

            return postOption.Match<IActionResult>(
                post =>
                    Ok(new PostResponse
                    {
                        Id = post.Id,
                        Name = post.Name,
                        UserId = post.UserId,
                        Tags = post.Tags.Select(p => new TagResponse {Name = p.Name})
                    }),
                NotFound);
        }

        [HttpPut(ApiRoutes.Posts.Update)]
        public IActionResult Update([FromRoute] Guid postId, [FromBody] UpdatePostRequest request)
        {
            bool userOwnsPost = this.postService.UserOwnsPost(postId, HttpContext.GetUserIdFromClaim());

            if (!userOwnsPost)
            {
                return BadRequest(new {error = "You do not own this post."});
            }

            Option<Post> postOption = this.postService.GetPostById(postId);
            return postOption.Match<IActionResult>(post =>
                {
                    post.Name = request.Name;
                    if (this.postService.UpdatePost(post))
                        return Ok(new PostResponse
                        {
                            Id = post.Id,
                            Name = post.Name,
                            UserId = post.UserId,
                            Tags = post.Tags.Select(p => new TagResponse {Name = p.Name})
                        });

                    return NotFound();
                },
                NotFound);
        }

        [HttpPost(ApiRoutes.Posts.Create)]
        public IActionResult Create([FromBody] CreatePostRequest postRequest)
        {
            Post post = this.postService.Create(postRequest.Name, postRequest.Tags, HttpContext.GetUserIdFromClaim());
            string location = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}/{ApiRoutes.Posts.Get.Replace("{postId}", post.Id.ToString())}";

            return Created(location, new PostResponse
            {
                Id = post.Id,
                Name = post.Name,
                UserId = post.UserId,
                Tags = post.Tags.Select(p => new TagResponse {Name = p.Name})
            });
        }

        [HttpDelete(ApiRoutes.Posts.Delete)]
        public IActionResult Delete([FromRoute] Guid postId)
        {
            bool userOwnsPost = this.postService.UserOwnsPost(postId, HttpContext.GetUserIdFromClaim());

            if (!userOwnsPost)
            {
                return BadRequest(new {error = "You do now own this post."});
            }

            bool deleted = this.postService.DeletePost(postId);

            if (deleted)
                return NoContent();

            return NotFound();
        }
    }
}