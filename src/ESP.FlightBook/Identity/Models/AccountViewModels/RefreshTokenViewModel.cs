using System.ComponentModel.DataAnnotations;

namespace ESP.FlightBook.Identity.Models.AccountViewModels
{
    public class RefreshTokenViewModel : AccountViewModel
    {
        [Required]
        public string AccessToken { get; set; }
        public string NewAccessToken { get; set; }
    }
}
