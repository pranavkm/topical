using System;
using System.Threading.Tasks;
using Topical.Models;

namespace Topical.Services
{
    public class CommentService : ICommentService
    {
        //private readonly TopicalContext _context;

        //public CommentService(TopicalContext context)
        //{
        //    _context = context;
        //}

        //public async Task Add(string topicId, string parentCommentId, Comment comment)
        //{
        //    var topic = await _context.Topics.FindAsync(topicId);
        //    if (topic == null)
        //    {
        //        throw new InvalidOperationException("Invalid topic id " + topicId);
        //    }

        //    topic.Comments.Add(comment);
        //    comment.Topic = topic;
        //    comment.ParentComment = new Comment { Id = parentCommentId };
        //    await _context.SaveChangesAsync();
        //}
    }
}