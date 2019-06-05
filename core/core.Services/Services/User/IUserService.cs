using core.Domain.Entities.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace core.Services.Services.User
{
    public interface IUserService
    {
        Task<bool> IsUsernameValid(string username);
        Task<IEnumerable<ApplicationUser>> GetUsers();
    }
}
