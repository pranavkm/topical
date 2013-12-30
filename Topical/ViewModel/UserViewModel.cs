using System;
using System.Collections.Generic;
using System.Linq;
using Topical.Models;

namespace Topical.ViewModel
{
    public class UserViewModel
    {
        public UserViewModel(User user)
        {
            Id = user.Id;
            CreatedOn = user.CreatedOn;
            Tags = user.FavoriteTags.ToList();
        }

        public string Id { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public List<string> Tags { get; set; }
    }
}