using System;
using Lucene.Net.Documents;

namespace Topical.Repository
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IndexAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets if an index is also stored. 
        /// </summary>
        public bool Store { get; set; }

        /// <summary>
        /// Gets or sets if an index is analyzed. 
        /// </summary>
        public bool Analyzed { get; set; }
    }
}