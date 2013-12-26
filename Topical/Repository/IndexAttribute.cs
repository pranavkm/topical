using System;
using Lucene.Net.Documents;

namespace Topical.Repository
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IndexAttribute : Attribute
    {
        public IndexAttribute()
        {
            Store = true;
        }

        /// <summary>
        /// Gets or sets if an index is analyzed. 
        /// </summary>
        public bool Analyzed { get; set; }

        /// <summary>
        /// Gets or sets if the index is stored.
        /// </summary>
        public bool Store { get; set; }
    }
}