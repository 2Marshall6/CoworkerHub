using CoworkerHub.Application.DTOs;
using CoworkerHub.Application.Exceptions;
using CoworkerHub.Application.Interfaces;
using CoworkerHub.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace CoworkerHub.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly TokenOptions _JwtOptions;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public TokenService(IOptions<TokenOptions> jwtOptions, IRefreshTokenRepository refreshTokenRepository)
        {
            _JwtOptions = jwtOptions.Value;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<AuthenticationDTO> GenerateJwt(Guid userId, string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, userName)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_JwtOptions.TokenLifetimeInMinutes),
                Issuer = _JwtOptions.Issuer,
                Audience = _JwtOptions.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JwtOptions.Key!)),
                    SecurityAlgorithms.HmacSha512Signature
                )
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthenticationDTO
            {
                UserName = userName,
                AccessToken = tokenHandler.WriteToken(token),
                ExpiresIn = _JwtOptions.TokenLifetimeInMinutes * 60, 
                RefreshToken = await GenerateRefreshToken(userId)
            };
        }

        public async Task<string> GenerateRefreshToken(Guid userId)
        {
            var refreshTokenValidDays = _JwtOptions.RefreshTokenLifetimeInDays;
            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenValidDays)
            };

            await _refreshTokenRepository.AddRefreshTokenAsync(refreshToken);

            return refreshToken.Token;
        }

        public async Task<Guid?> ValidateRefreshToken(string token)
        {
            var refreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(token);
            if (refreshToken == null || refreshToken.ExpiresAt < DateTime.UtcNow)
            {
                throw new UnauthorizedException("Refresh token is expired");
            }

            await _refreshTokenRepository.DeleteRefreshTokenAsync(token);

            return refreshToken.UserId;
        }
    }
}
