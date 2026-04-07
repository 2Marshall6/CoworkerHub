using CoworkerHub.Application.DTOs.Authentication;

namespace CoworkerHub.Application.Interfaces
{
    public interface ITokenService
    {
        Task<AuthenticationDTO> GenerateJwt(Guid userId, string userName);
        Task<Guid?> ValidateRefreshToken(string refreshToken);
        Task<string> GenerateRefreshToken(Guid userId);
    }
}
