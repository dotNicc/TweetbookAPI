using Microsoft.AspNetCore.Mvc;

namespace TweetBook.Controllers
{
    public class TestController : Controller
    {
        [HttpGet("/api/user")]
        public IActionResult Get()
        {
            return Ok(new {name = "Nic"});
        }
    }
}