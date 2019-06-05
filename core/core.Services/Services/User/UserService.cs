using core.Data.Repositories.User;
using core.Domain.Entities.User;
using core.Services.Services.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace core.Services.Services.User
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly ITextService _textService;

        public UserService(IUserRepository repo, ITextService textService)
        {
            _repo = repo;
            _textService = textService;
        }

        public async Task<bool> IsUsernameValid(string username)
        {
            if (username.Length >= 4 && username.Length <= 30) //username chatracter min and max
            {
                if (username != "null")
                {
                    var usernameSlug = _textService.GenerateSlug(username);
                    if (username == usernameSlug) // only allow alpha-numeric usernames
                    {
                        return await _repo.IsUsernameAvailable(username);
                    }
                }
            }

            return false;
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsers()
        {
            return await _repo.GetAllUsers();
        }
    }
}
