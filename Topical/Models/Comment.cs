using System.Collections.Generic;

namespace Topical.Models
{
    public class Comment
    {
        public string Id { get; set; }

        public Comment ParentComment { get; set; }

        public Topic Topic { get; set; }

        public string Text { get; set; }

        public List<Comment> Comments { get; set; }
    }
}
