using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Topical.Models
{
    public class FilterCriteria
    {
        private static readonly FilterCriteria _default = new FilterCriteria { Count = 10 };

        public static FilterCriteria Default
        {
            get { return _default; }
        }

        [Range(1, 50)]
        public int Count { get; set; }

        public IEnumerable<string> Tags { get; set; }
    }
}