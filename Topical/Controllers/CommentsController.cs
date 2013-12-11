using System;
using System.Threading.Tasks;
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

        //public async Task<IHttpActionResult> Add(string topicId, string parentCommentId, Comment comment)
        //{
        //    if (String.IsNullOrEmpty(topicId))
        //    {
        //        return BadRequest("Missing topicId");
        //    }

        //    if (String.IsNullOrEmpty(comment.Text))
        //    {
        //        return BadRequest("Missing comment text");
        //    }

        //    await _commentService.Add(topicId, parentCommentId, comment);
        //    return Ok(comment);
        //}
    }
}