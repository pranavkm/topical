using System.Collections.Generic;
using System.Threading.Tasks;
using Topical.Models;

namespace Topical.Services
{
    public interface ICommentService
    {
        void Create(Comment comment);

        IEnumerable<Comment> GetComments(string topicId, int n);
    }
}
