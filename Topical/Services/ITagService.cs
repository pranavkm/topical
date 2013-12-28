using System.Collections.Generic;
using Topical.Models;

namespace Topical.Services
{
    public interface ITagService
    {
        IEnumerable<TopicTag> GetTags(string topicId);

        void AddTags(IEnumerable<TopicTag> topicTags);

        TopicTag AddTagVote(string topicId, string tagId, int vote);
    }
}
