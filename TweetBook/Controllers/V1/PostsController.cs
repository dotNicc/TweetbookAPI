﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TweetBook.Contract.V1;
using TweetBook.Domain;

namespace TweetBook.Controllers.V1
{
    public class PostsController : Controller
    {
        private readonly List<Post> posts;

        public PostsController()
        {
            this.posts = new List<Post>();
            for (int i = 0; i < 5; i++)
            {
                this.posts.Add(new Post {Id = Guid.NewGuid().ToString()});
            }
        }
        
        [HttpGet(ApiRoutes.Posts.GetAll)]
        public IActionResult GetAll()
        {
            return Ok(this.posts);
        }
    }
}