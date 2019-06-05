using core.Data.Contexts;
using core.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace core.Data.Repositories.Auth
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Client
        public async Task<AuthClient> GetClient(int clientId)
        {
            return await _context.AuthClients.FirstOrDefaultAsync(c => c.Id == clientId);
        }

        // Refresh Tokens
        public async Task<RefreshToken> GetRefreshToken(Guid tokenUnique, int clientId)
        {
            return await _context.RefreshTokens.Where(t => t.Value == tokenUnique && t.ClientId == clientId).Include(t => t.User).FirstOrDefaultAsync();
        }

        public async Task<bool> AddRefreshToken(RefreshToken refreshToken)
        {
            _context.Add(refreshToken);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        {
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Id == refreshToken.Id);

            if (token != null)
            {
                _context.RefreshTokens.Remove(token);

                return await _context.SaveChangesAsync() > 0;
            }

            return false;
        }

        public async Task<bool> RemoveRefreshToken(Guid tokenUnique)
        {
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Value == tokenUnique);

            if (token != null)
            {
                _context.RefreshTokens.Remove(token);

                return await _context.SaveChangesAsync() > 0;
            }

            return false;
        }

        // Disposing
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
