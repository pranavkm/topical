using System;
using Topical.Models;
using Topical.Repository;

namespace Topical.Services
{
    public class CommentService : ICommentService
    {
        private readonly LuceneProvider _dbProvider;

        public CommentService(LuceneProvider context)
        {
            _dbProvider = context;
        }

        public void Create(Comment comment)
        {
            comment.Id = IdProvider.GenerateId();
            comment.CreatedDate = DateTimeOffset.UtcNow;
            comment.LastModifiedDate = DateTimeOffset.UtcNow;
            _dbProvider.AddRecord(comment);
        }
    }
}