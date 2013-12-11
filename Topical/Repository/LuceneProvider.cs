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

        public IEnumerable<TEntity> GetRecords<TEntity>(Query query = null, int n = 20)
        {
            var reader = IndexReader.Open(GetIndexDirectory<TEntity>(), readOnly: true);
            using (var indexSearcher = new IndexSearcher(reader))
            {
                Query nullQuery = new QueryParser(_version, "Id", new StandardAnalyzer(_version)).Parse("*:*");
                TopDocs docs = indexSearcher.Search(query ?? nullQuery, n);
                return docs.ScoreDocs.Select(d => {
                    Document doc = reader.Document(d.Doc);
                    return _entityMapper.MapFromDocument<TEntity>(doc);
                });
            }
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