using Topical.Repository;

namespace Topical.Models
{
    public class TopicTag
    {
        [Index(Analyzed=false, Store=false)]
        public string TopicId { get; set; }

        [Index(Analyzed = true, Store = false)]
        public string TagId { get; set; }

        public int Tally { get; set; }

        public int Fit { get; set; }

        public int Unfit { get; set; }
    }
}