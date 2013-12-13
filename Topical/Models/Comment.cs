using System;
using System.Collections.Generic;
using Topical.Repository;

namespace Topical.Models
{
    public class Comment
    {
        [Index]
        public string Id { get; set; }

        [Index]
        public string TopicId { get; set; }

        [Index]
        public string ParentId { get; set; }

        [Index]
        public List<string> AncestorIds { get; set; }

        public string Text { get; set; }

        public int Depth { get; set; }

        [Index]
        public DateTimeOffset CreatedDate { get; set; }
        
        public DateTimeOffset LastModifiedDate { get; set; }
    }
}
