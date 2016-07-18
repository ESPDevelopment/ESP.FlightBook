using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ESP.FlightBook.Identity.Models.AccountViewModels
{
    public class AccountViewModel
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public List<IdentityError> Errors { get; set; }
    }
}
