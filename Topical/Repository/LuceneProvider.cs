using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;

namespace Topical.Repository
{
    public class LuceneProvider : System.IDisposable
    {
        private static readonly Lucene.Net.Util.Version _version = Lucene.Net.Util.Version.LUCENE_30;
        private static ConcurrentDictionary<Type, IndexWriter> _indexWriters = new ConcurrentDictionary<Type, IndexWriter>();
        private readonly EntityMapper _entityMapper;
        private readonly string _rootDir;

        public LuceneProvider(string directory)
        {
            _entityMapper = new EntityMapper();
            _rootDir = directory;
        }

        public void AddRecord<TEntity>(TEntity entity)
        {
            IndexWriter writer = EnsureWriter<TEntity>();
            Document document = _entityMapper.MapToDocument(entity);
            writer.AddDocument(document);
            writer.Commit();
        }

        public TEntity GetRecord<TEntity>(string id)
        {
            Query query = new TermQuery(new Term("Id", id));
            var reader = IndexReader.Open(GetIndexDirectory<TEntity>(), readOnly: true);
            using (var indexSearcher = new IndexSearcher(reader))
            {
                TopDocs docs = indexSearcher.Search(query, 1);
                if (docs.TotalHits > 0)
                {
                    Document doc = reader.Document(docs.ScoreDocs[0].Doc);
                    return _entityMapper.MapFromDocument<TEntity>(doc);
                }
            }

            return default(TEntity);
        }

        public IEnumerable<TEntity> GetRecords<TEntity>(int n)
        {
            return GetRecords<TEntity>("*:*", n);
        }

        public IEnumerable<TEntity> GetRecords<TEntity>(string query, int n)
        {
            Query parsedQuery = new QueryParser(_version, "Id", new StandardAnalyzer(_version)).Parse(query);
            return GetRecords<TEntity>(parsedQuery, n);
        }

        public IEnumerable<TEntity> GetRecords<TEntity>(Query query, int n)
        {
            var directory = GetIndexDirectory<TEntity>();
            if (!IndexReader.IndexExists(directory))
            {
                return Enumerable.Empty<TEntity>();
            }

            var reader = IndexReader.Open(directory, readOnly: true);
            using (var indexSearcher = new IndexSearcher(reader))
            {
                TopDocs docs = indexSearcher.Search(query, n);
                return docs.ScoreDocs.Select(d =>
                {
                    Document doc = reader.Document(d.Doc);
                    return _entityMapper.MapFromDocument<TEntity>(doc);
                });
            }
        }

        public void UpdateRecord<TEntity>(TEntity entity)
        {
            IndexWriter writer = EnsureWriter<TEntity>();
            Document document = _entityMapper.MapToDocument(entity);
            writer.DeleteDocuments(GetLookupQuery(entity));
            writer.AddDocument(document);
            writer.Commit();
        }

        public Query GetLookupQuery<TEntity>(TEntity entity)
        {
            return _entityMapper.GetLookupTerm(entity);
        }

        private IndexWriter EnsureWriter<TEntity>()
        {
            return _indexWriters.GetOrAdd(typeof(TEntity), _ =>
            {
                var fsDirectory = GetIndexDirectory<TEntity>();
                return new IndexWriter(fsDirectory, new StandardAnalyzer(_version), mfl: IndexWriter.MaxFieldLength.UNLIMITED);
            });
        }

        private SimpleFSDirectory GetIndexDirectory<TEntity>()
        {
            var directory = new DirectoryInfo(Path.Combine(_rootDir, typeof(TEntity).Name));
            directory.Create();
            var fsDirectory = new SimpleFSDirectory(directory);
            return fsDirectory;
        }

        public void Dispose()
        {
            foreach (var item in _indexWriters)
            {
                item.Value.Dispose();
            }
        }
    }
}