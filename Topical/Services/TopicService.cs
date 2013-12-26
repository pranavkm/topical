using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Topical.Models;
using Topical.Repository;

namespace Topical.Services
{
    public class TopicService : ITopicService
    {
        private readonly LuceneProvider _dbProvider;
        private readonly ITagService _tagService;

        public TopicService(LuceneProvider context, ITagService tagService)
        {
            _dbProvider = context;
            _tagService = tagService;    
        }

        public void Create(Topic topic)
        {
            topic.Id = IdProvider.GenerateId();
            topic.CreatedOn = DateTimeOffset.UtcNow;
            topic.LastModifiedOn = DateTimeOffset.UtcNow;
            _dbProvider.AddRecord(topic);

            var tags = topic.Tags.Select(tag => new TopicTag { TagId = tag, TopicId = topic.Id });
            _tagService.AddTags(tags);
        }

        public virtual Topic GetTopic(string id)
        {
            return _dbProvider.GetRecord<Topic>(id);
        }

        public virtual IEnumerable<Topic> GetTopics(TopicFilter topicFilter)
        {
            Query query;
            if (topicFilter.Tags != null && topicFilter.Tags.Any())
            {
                var booleanQuery = new BooleanQuery();
                foreach (var tag in topicFilter.Tags)
                {
                    booleanQuery.Add(new TermQuery(new Term("Tags", tag)), Occur.MUST);
                }
                query = booleanQuery;
            }
            else
            {
                query = new MatchAllDocsQuery();
            }
            return _dbProvider.GetRecords<Topic>(query, n : 25);
        }
    }
}