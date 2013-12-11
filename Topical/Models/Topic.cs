using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Topical.Repository;

namespace Topical.Models
{
    public class Topic
    {
        [IndexMetadata(IsKey=true)]
        public string Id { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public string Description { get; set; }

        public List<string> Tags { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset LastModifiedOn { get; set; }
    }
}