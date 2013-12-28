using System;
using System.Collections.Generic;
using Lucene.Net.QueryParsers;
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
            if (!String.IsNullOrEmpty(comment.ParentId))
            {
                Comment parentComment = _dbProvider.GetRecord<Comment>(comment.ParentId);
                if (parentComment == null)
                {
                    comment.ParentId = null;
                }
                else
                {
                    comment.AncestorIds = parentComment.AncestorIds;
                    comment.AncestorIds.Add(parentComment.Id);
                }
            }

            _dbProvider.AddRecord(comment);
        }

        public IEnumerable<Comment> GetComments(string topicId, int n)
        {
            string query = "TopicId:\"" + QueryParser.Escape(topicId) + '"';
            return _dbProvider.GetRecords<Comment>(query, n);
        }
    }
}