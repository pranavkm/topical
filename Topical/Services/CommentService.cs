using System;
using System.Collections.Generic;
using System.Web.Http;
using Lucene.Net.Index;
using Lucene.Net.Search;
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

        [Authorize]
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
            Query query = new TermQuery(new Term("TopicId", topicId));
            return _dbProvider.GetRecords<Comment>(query, n);
        }
    }
}