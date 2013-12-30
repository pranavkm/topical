using System.ComponentModel.DataAnnotations;

namespace Topical.ViewModel
{
    public class RegisterUserViewModel
    {
        [Required]
        [MinLength(4)]
        public string UserName { get; set; }

        [Required]
        [MinLength(4)]
        public string Password { get; set; }
    }
}