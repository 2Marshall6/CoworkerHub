using CoworkerHub.Application.DTOs.Authentication;

namespace CoworkerHub.Application.Interfaces
{
    public interface ITokenService
    {
        Task<AuthenticationDTO> GenerateJwt(Guid userId, string userName, IList<string> roles);
        Task<Guid?> ValidateRefreshToken(string refreshToken);
        Task<string> GenerateRefreshToken(Guid userId);
    }
}
