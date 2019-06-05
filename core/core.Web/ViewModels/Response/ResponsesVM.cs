using core.Domain.Entities.User;
using System.Collections.Generic;
using System.Linq;

namespace core.Web.ViewModels.Response
{
    public class ResponsesVM
    {
        public List<UserResponseVM> ResponseList { get; set; }

        public ResponsesVM(List<ApplicationUser> users)
        {
            ResponseList = getResponses(users);
        }

        private List<UserResponseVM> getResponses(List<ApplicationUser> users)
        {
            if (users.Count() > 0)
            {
                return users.Select(u => new UserResponseVM(u)).ToList();
            }

            return new List<UserResponseVM>();
        }
    }
}
