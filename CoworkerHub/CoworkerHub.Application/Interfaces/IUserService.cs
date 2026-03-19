using CoworkerHub.Application.DTOs.Authentication;

namespace CoworkerHub.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> RegisterUserAsync(RegisterUserDTO createModel, CancellationToken cancellationToken);
        Task<AuthenticationDTO> LoginUserAsync(LoginUserDTO loginModel);
        Task ChangePasswordAsync(ChangeUserPasswordDTO changeUserPasswordDTO, CancellationToken cancellationToken);
        Task<AuthenticationDTO> RefreshTokensAsync(string oldRefreshToken);
    }
}
