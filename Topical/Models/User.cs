using System;
using System.Collections.Generic;
using Topical.Repository;

namespace Topical.Models
{
    public class User
    {
        [Index(Analyzed = false)]
        public string Id { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        [Index(Analyzed = false)]
        public string PasswordHash { get; set; }

        public List<string> FavoriteTags { get; set; }
    }
}