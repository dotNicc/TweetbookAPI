using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TweetBook.Contract.V2.Responses;
using TweetBook.Services;

namespace TweetBook.Controllers.V2
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Poster,Admin")]
    public class TagsController : Controller
    {
        private readonly IPostService postService;

        public TagsController(IPostService postService)
        {
            this.postService = postService;
        }
        
        [HttpGet(Contract.V2.ApiRoutes.Tags.GetAll)]
        [Authorize(Policy = "TagViewer")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(this.postService.GetAllTags().Select(tag => new TagResponse
            {
                Name = tag.Name,
                TagType = tag.TagType
            }));
        }
    }
}