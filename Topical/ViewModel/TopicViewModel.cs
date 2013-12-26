using System;
using System.Collections.Generic;
using Topical.Models;

namespace Topical.ViewModel
{
    public class TopicViewModel
    {
        public TopicViewModel()
        {

        }

        public TopicViewModel(Topic topic)
        {
            Id = topic.Id;
            Title = topic.Title;
            Description = topic.Description;
            Url = topic.Url;
            CreatedOn = topic.CreatedOn;
            LastModifiedOn = topic.LastModifiedOn;
            Tags = topic.Tags;
        }

        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset LastModifiedOn { get; set; }

        public List<string> Tags { get; set; }
    }
}