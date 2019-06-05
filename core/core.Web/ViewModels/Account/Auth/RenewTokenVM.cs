using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace core.Web.ViewModels.Account.Auth
{
    public class RenewTokenVM
    {
        [Required]
        public string RefreshToken { get; set; }
        [Required]
        public int ClientId { get; set; }
        [Required]
        public string GrantType { get; set; }
        [Required]
        public string Scope { get; set; }

        public RenewTokenVM()
        {

        }
    }
}
