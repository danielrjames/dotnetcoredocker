using core.Data.Contexts;
using core.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace core.Data.Repositories.User
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsUsernameAvailable(string username)
        {
            var result = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);

            if (result != null)
            {
                return false;
            }

            return true;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        //disposing
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
