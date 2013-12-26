using System.Collections.Generic;
using Topical.Models;

namespace Topical.Services
{
    public interface ITagService
    {
        void AddTags(IEnumerable<TopicTag> topicTags);

        void AddTagVote(TopicTag topicTag, int vote);
    }
}
