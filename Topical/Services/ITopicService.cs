using System.Collections.Generic;
using System.Threading.Tasks;
using Topical.Models;

namespace Topical.Services
{
    public interface ITopicService
    {
        void Create(Topic topic);

        IEnumerable<Topic> GetTopics();
    }
}
