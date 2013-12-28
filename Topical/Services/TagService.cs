using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Topical.Models;
using Topical.Repository;

namespace Topical.Services
{
    public class TagService : ITagService
    {
        private readonly LuceneProvider _dbProvider;

        public TagService(LuceneProvider context)
        {
            _dbProvider = context;
        }


        public IEnumerable<TopicTag> GetTags(string topicId)
        {
            var query = new TermQuery(new Term("TopicId", topicId));
            return _dbProvider.GetRecords<TopicTag>(query, n: 100);
        }

        public void AddTags(IEnumerable<TopicTag> topicTags)
        {
            Parallel.ForEach(topicTags, tag =>
            {
                _dbProvider.AddRecord<TopicTag>(tag);
            });
        }

        public TopicTag AddTagVote(string topicId, string tagId, int vote)
        {
            var tagQuery = GetLookupQuery(topicId, tagId);
            var topicTag = _dbProvider.GetRecord<TopicTag>(tagQuery);
            
            if (topicTag == null)
            {
                return null;
            }

            vote = vote >= 0 ? 1 : -1;
            if (vote < 0)
            {
                topicTag.Unfit -= 1;
                topicTag.Tally -= 1;
            }
            else
            {
                topicTag.Fit += 1;
                topicTag.Tally += 1;
            }

            _dbProvider.UpdateRecord(topicTag);

            return topicTag;
        }

        private Query GetLookupQuery(string topicId, string tagId)
        {
            var query = new BooleanQuery();
            query.Add(new TermQuery(new Term("TopicId", topicId)), Occur.MUST);

            if (!String.IsNullOrEmpty(tagId))
            {
                query.Add(new TermQuery(new Term("TagId", tagId)), Occur.MUST);
            }

            return query;
        }
    }
}