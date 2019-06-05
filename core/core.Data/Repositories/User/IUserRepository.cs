using core.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace core.Data.Repositories.User
{
    public interface IUserRepository : IDisposable
    {
        Task<bool> IsUsernameAvailable(string username);
        Task<IEnumerable<ApplicationUser>> GetAllUsers();
    }
}
