using System.ComponentModel.DataAnnotations;

namespace ESP.FlightBook.Identity.Models.AccountViewModels
{
    public class LoginViewModel : AccountViewModel
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string AccessToken { get; set; }
    }
}
