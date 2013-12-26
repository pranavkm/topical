using System;
using System.Collections.Generic;
using Topical.Models;

namespace Topical.ViewModel
{
    public class CommentViewModel
    {
        public TopicViewModel Topic { get; set; }

        public CommentViewModel Parent { get; set; }

        public List<CommentViewModel> Children { get; set; }

        public string Id { get; set; }

        public string Text { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset LastModifiedOn { get; set; }

        public static List<CommentViewModel> CreateCommentTree(string topicId, IEnumerable<Comment> comments)
        {
            var topic = new TopicViewModel { Id = topicId };
            var rootComments = new List<CommentViewModel>();
            var commentLookup = new Dictionary<string, CommentViewModel>(StringComparer.OrdinalIgnoreCase);
            foreach (var comment in comments)
            {
                CommentViewModel commentModel;
                if (!commentLookup.TryGetValue(comment.Id, out commentModel))
                {
                    commentModel = new CommentViewModel();
                }
                
                commentModel.Topic = topic;
                commentModel.Id = comment.Id;
                commentModel.Text = comment.Text;
                commentModel.LastModifiedOn = comment.LastModifiedDate;
                commentModel.CreatedOn = comment.CreatedDate;
                
                if (comment.ParentId != null)
                {
                    CommentViewModel parentComment;
                    if (!commentLookup.TryGetValue(comment.ParentId, out parentComment))
                    {
                        parentComment = new CommentViewModel();
                        commentLookup.Add(comment.ParentId, parentComment);
                    }

                    if (parentComment.Children != null)
                    {
                        parentComment.Children = new List<CommentViewModel>();
                    }
                    parentComment.Children.Add(commentModel);
                }
                else
                {
                    // The comment is a top-level comment
                    rootComments.Add(commentModel);
                }
            }
            return rootComments;
        }
    }
}