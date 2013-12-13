using System.Threading.Tasks;
using Topical.Models;

namespace Topical.Services
{
    public interface ICommentService
    {
        void Create(Comment comment);
    }
}
