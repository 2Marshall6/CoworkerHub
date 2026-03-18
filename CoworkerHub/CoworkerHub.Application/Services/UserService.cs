using AutoMapper;
using CoworkerHub.Application.DTOs;
using CoworkerHub.Application.Exceptions;
using CoworkerHub.Application.Interfaces;
using CoworkerHub.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CoworkerHub.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public UserService(UserManager<User> userManager, ITokenService jwtService, IMapper mapper)
        {
            _userManager = userManager;
            _tokenService = jwtService;
            _mapper = mapper;
        }

        public async Task<UserDTO> RegisterUserAsync(RegisterUserDTO createModel, CancellationToken cancellationToken)
        {
            User user = _mapper.Map<User>(createModel);

            var result = await _userManager.CreateAsync(user, createModel.Password);

            if (!result.Succeeded)
            {
                throw new AppValidationException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<AuthenticationDTO> LoginUserAsync(LoginUserDTO loginModel)
        {
            var user = await _userManager.FindByEmailAsync(loginModel.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                throw new UnauthorizedException("Wrong password or email.");
            }

            return await _tokenService.GenerateJwt(user.Id, user.UserName);
        }
        public async Task<AuthenticationDTO> RefreshTokensAsync(string oldRefreshToken)
        {
            var userId = await _tokenService.ValidateRefreshToken(oldRefreshToken);

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            var newAuthData = await _tokenService.GenerateJwt(user.Id, user.UserName);

            return newAuthData;
        }

        public async Task ChangePasswordAsync(ChangeUserPasswordDTO changeUserPasswordModel, CancellationToken cancellation)
        {
            var user = await _userManager.FindByEmailAsync(changeUserPasswordModel.Email);

            var result = await _userManager.ChangePasswordAsync(
                user,
                changeUserPasswordModel.CurrentPassword,
                changeUserPasswordModel.NewPassword
            );

            if (!result.Succeeded)
            {
                throw new AppValidationException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}
