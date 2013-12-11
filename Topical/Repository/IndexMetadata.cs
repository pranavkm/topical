using System;
using Lucene.Net.Documents;

namespace Topical.Repository
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IndexMetadata : Attribute
    {
        public Field.Index Index { get; set; }

        public Field.Store Store { get; set; }

        public bool IsKey { get; set; }
    }
}