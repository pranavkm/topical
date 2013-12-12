using System;
using System.Collections.Generic;
using System.Web.Http;
using Lucene.Net.Index;
using Lucene.Net.Search;
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
            topic.CreatedOn = DateTimeOffset.UtcNow;
            topic.LastModifiedOn = DateTimeOffset.UtcNow;
            _dbProvider.AddRecord(topic);
        }

        public virtual Topic GetTopic(string id)
        {
            return _dbProvider.GetRecord<Topic>(id);
        }

        public virtual IEnumerable<Topic> GetTopics()
        {
            return _dbProvider.GetRecords<Topic>();
        }
    }
}