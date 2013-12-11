using System.Collections.Generic;
using System.Web.Http;
using Topical.Models;
using Topical.Repository;

namespace Topical.Services
{
    public class TopicService : ITopicService
    {
        private readonly LuceneProvider _dbProvider;

        public TopicService(LuceneProvider context)
        {
            _dbProvider = context;
        }

        public void Create(Topic topic)
        {
            topic.Id = IdProvider.GenerateTopicId();
            _dbProvider.AddRecord(topic);
        }

        public virtual Topic GetTopic(string id)
        {
            return null;
        }

        public virtual IEnumerable<Topic> GetTopics()
        {
            return _dbProvider.GetRecords<Topic>();
        }
    }
}