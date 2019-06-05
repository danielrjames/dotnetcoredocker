using core.Domain.Entities.Auth;
using System;
using System.Threading.Tasks;

namespace core.Data.Repositories.Auth
{
    public interface IAuthRepository : IDisposable
    {
        Task<AuthClient> GetClient(int clientId);

        Task<RefreshToken> GetRefreshToken(Guid tokenUnique, int clientId);
        Task<bool> AddRefreshToken(RefreshToken refreshToken);
        Task<bool> RemoveRefreshToken(RefreshToken refreshToken);
        Task<bool> RemoveRefreshToken(Guid tokenUnique);
    }
}
