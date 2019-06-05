using core.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Web.ViewModels.Response
{
    public class UserResponseVM
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Response { get; set; }
        public bool Submitted { get; set; }

        public UserResponseVM(ApplicationUser user)
        {
            Name = user.Name;
            Username = user.UserName;
            Response = user.Response;
            Submitted = user.Submitted;
        }
    }
}
