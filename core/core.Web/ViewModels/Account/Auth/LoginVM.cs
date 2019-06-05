using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace core.Web.ViewModels.Account.Auth
{
    public class LoginVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public int ClientId { get; set; }
        [Required]
        public string GrantType { get; set; }
        public string Scope { get; set; }
        public bool RememberMe { get; set; }

        public LoginVM()
        {

        }
    }
}
