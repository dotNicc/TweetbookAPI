using System;
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
            return Ok(this.postService.GetPosts());
        }
        
        [HttpGet(ApiRoutes.Posts.Get)]
        public IActionResult Get([FromRoute]Guid postId)
        {
            Post post = this.postService.GetPostById(postId);

            if (post == null) 
                return NotFound();
            
            return Ok(post);
        }
        
        [HttpPut(ApiRoutes.Posts.Update)]
        public IActionResult Update([FromRoute]Guid postId, [FromBody] UpdatePostRequest request)
        {
            bool userOwnsPost = this.postService.UserOwnsPost(postId, HttpContext.GetUserIdFromClaim());

            if (!userOwnsPost)
            {
                return BadRequest(new {error = "You do now own this post."});
            }
            
            Post post = this.postService.GetPostById(postId);
            post.Name = request.Name;

            if (this.postService.UpdatePost(post))
                return Ok(post);
            
            return NotFound();
        }

        [HttpPost(ApiRoutes.Posts.Create)]
        public IActionResult Create([FromBody] CreatePostRequest postRequest)
        {
            Post post = this.postService.Create(postRequest.Id, HttpContext.GetUserIdFromClaim());
            string location = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}/{ApiRoutes.Posts.Get.Replace("{postId}", post.Id.ToString())}";

            var response = new PostResponse() {Id = post.Id};
            return Created(location, response);
        }
        
        [HttpDelete(ApiRoutes.Posts.Delete)]
        public IActionResult Delete([FromRoute]Guid postId)
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