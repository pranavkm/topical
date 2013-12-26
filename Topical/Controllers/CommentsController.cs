using System;
using System.Collections.Generic;
using System.Web.Http;
using Topical.Models;
using Topical.Services;
using Topical.ViewModel;

namespace Topical.Controllers
{
    [RoutePrefix("api/topics/{topicId}")]
    public class CommentsController : ApiController
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [Route("comments")]
        public IHttpActionResult CreateComment(Comment comment)
        {
            if (String.IsNullOrEmpty(comment.TopicId))
            {
                return BadRequest("TopicId is missing");
            }

            _commentService.Create(comment);
            return Ok(comment);
        }

        [Route("comments")]
        public IEnumerable<CommentViewModel> GetComments(string topicId)
        {
            var comments = _commentService.GetComments(topicId, n: Int32.MaxValue);
            return CommentViewModel.CreateCommentTree(topicId, comments);
        }
    }
}