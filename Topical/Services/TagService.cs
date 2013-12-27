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

        public void AddTagVote(TopicTag topicTag, int vote)
        {
            var tagQuery = _dbProvider.GetLookupQuery(topicTag);
            var currentTag = _dbProvider.GetRecords<TopicTag>(tagQuery, n: 1)
                                        .FirstOrDefault();

            if (currentTag == null)
            {
                return;
            }
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
        }
    }
}