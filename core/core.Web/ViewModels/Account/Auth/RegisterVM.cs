using System.ComponentModel.DataAnnotations;

namespace core.Web.ViewModels.Account.Auth
{
    public class RegisterVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public int ClientId { get; set; }
        [Required]
        public string GrantType { get; set; }
        public string Scope { get; set; }

        public RegisterVM()
        {

        }
    }
}
