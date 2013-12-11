using Topical.Repository;

namespace Topical.Models
{
    public class TopicTag
    {
        [IndexMetadata(IsKey = true)]
        public string Tag { get; set; }
    }
}