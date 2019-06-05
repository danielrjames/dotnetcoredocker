using core.Data.Repositories.Auth;
using core.Domain.Entities.Auth;
using core.Domain.Entities.User;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace core.Services.Services.Auth
{
    public class TokenService : ITokenService
    {
        private readonly TokenConfig _tokenConfig;
        private readonly IAuthRepository _repo;

        public TokenService(
            IOptions<TokenConfig> tokenConfig,
            IAuthRepository repo
        )
        {
            _tokenConfig = tokenConfig.Value;
            _repo = repo;
        }

        public async Task<Object> GetTokensAsync(ApplicationUser user, int clientId, string scope, bool rememberMe)
        {
            var valid = await IsValidClient(clientId);

            if (valid) //check if client is valid when there are multiple clients
            {
                var now = DateTime.UtcNow;
                var jwtExpiration = TimeSpan.FromMinutes(30);
                var refreshExpiration = rememberMe ? TimeSpan.FromDays(7) : TimeSpan.FromHours(2);

                var response = new TokenResponse
                {
                    AccessToken = CreateJwtToken(user, now, jwtExpiration, refreshExpiration),
                    TokenType = "Bearer",
                    Expiration = (int)jwtExpiration.TotalSeconds
                };

                if (scope == "offline_access")
                {
                    var refreshToken = await CreateRefreshToken(user, clientId, now, refreshExpiration);

                    if (refreshToken != null)
                    {
                        return new
                        {
                            access_token = response.AccessToken,
                            expires_in = response.Expiration,
                            token_type = response.TokenType,
                            refresh_token = refreshToken
                        };
                    }
                }

                return new
                {
                    access_token = response.AccessToken,
                    expires_in = response.Expiration,
                    token_type = response.TokenType
                };
            }

            return null;
        }

        public async Task<Object> RenewTokensAsync(Guid oldToken, int clientId, string scope)
        {
            var token = await _repo.GetRefreshToken(oldToken, clientId);

            if (token != null)
            {
                var result = await _repo.RemoveRefreshToken(token);

                if (result)
                {
                    if (token.ExpiresUtc > DateTime.UtcNow)
                    {
                        var user = token.User;

                        if (user != null)
                        {
                            bool rememberMe = false;

                            if (token.IssuedUtc.Date.AddDays(30) == token.ExpiresUtc.Date)
                            {
                                rememberMe = true;
                            }

                            return await GetTokensAsync(user, clientId, scope, rememberMe);
                        }
                    }
                }
            }

            return null;
        }

        public async Task<bool> RevokeTokenAsync(Guid tokenUnique)
        {
            return await _repo.RemoveRefreshToken(tokenUnique);
        }

        private async Task<bool> IsValidClient(int clientId)
        {
            var model = await _repo.GetClient(clientId);

            if (model != null)
            {
                return true;
            }

            return false;
        }

        private string CreateJwtToken(ApplicationUser user, DateTime now, TimeSpan expiration, TimeSpan refreshExpiration)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenConfig.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // add claims here
            var jwtClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUniversalTime().ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim("rExp", new DateTimeOffset(now).ToUniversalTime().Add(refreshExpiration).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim("name", user.Name, ClaimValueTypes.String),
                new Claim("submitStatus", user.Submitted.ToString(), ClaimValueTypes.Boolean)
        };

            var token = new JwtSecurityToken(
                issuer: _tokenConfig.Issuer,
                audience: _tokenConfig.Audience,
                claims: jwtClaims,
                notBefore: now,
                expires: now.Add(expiration),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<string> CreateRefreshToken(ApplicationUser user, int clientId, DateTime now, TimeSpan expiration)
        {
            var refreshToken = new RefreshToken
            {
                Value = Guid.NewGuid(),
                ClientId = clientId,
                IssuedUtc = now,
                ExpiresUtc = now.Add(expiration),
                UserId = user.Id
            };

            var result = await _repo.AddRefreshToken(refreshToken);

            if (result)
            {
                return refreshToken.Value.ToString("n");
            }

            return null;
        }
    }
}
