using core.Domain.Entities.User;
using System;
using System.Threading.Tasks;

namespace core.Services.Services.Auth
{
    public interface ITokenService
    {
        Task<Object> GetTokensAsync(ApplicationUser user, int clientId, string scope, bool rememberMe);
        Task<Object> RenewTokensAsync(Guid oldToken, int clientId, string scope);
        Task<bool> RevokeTokenAsync(Guid tokenUnique);
    }
}
