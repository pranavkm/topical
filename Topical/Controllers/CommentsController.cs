using System;
using System.Collections.Generic;
using System.Web.Http;
using Topical.Models;
using Topical.Services;

namespace Topical.Controllers
{
    public class CommentsController : ApiController
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public IHttpActionResult CreateComment(Comment comment)
        {
            if (String.IsNullOrEmpty(comment.TopicId))
            {
                return BadRequest("TopicId is missing");
            }

            _commentService.Create(comment);
            return Ok(comment);
        }

        public IEnumerable<Comment> GetComments(string topicId)
        {
            return _commentService.GetComments(topicId, n: Int32.MaxValue);
        }
    }
}