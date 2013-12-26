using System;
using System.Collections.Generic;
using Topical.Repository;

namespace Topical.Models
{
    public class Topic
    {
        [Index]
        public string Id { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public string Description { get; set; }

        [Index(Analyzed = false)]
        public List<string> Tags { get; set; }

        [Index]
        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset LastModifiedOn { get; set; }
    }
}