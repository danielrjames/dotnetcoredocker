using System.ComponentModel.DataAnnotations;

namespace core.Web.ViewModels.Account.Auth
{
    public class RevokeTokenVM
    {
        [Required]
        public string RefreshToken { get; set; }

        public RevokeTokenVM()
        {

        }
    }
}
