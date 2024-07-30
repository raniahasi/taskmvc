using System.ComponentModel.DataAnnotations;

namespace WeCart.ViewModels
{
    public class ProfileViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string ProfileImage { get; set; }
    }
}
