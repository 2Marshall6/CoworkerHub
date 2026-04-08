using AutoMapper;
using CoworkerHub.Application.DTOs.Authentication;
using CoworkerHub.Application.Exceptions;
using CoworkerHub.Application.Interfaces;
using CoworkerHub.Domain.Entities;
using CoworkerHub.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace CoworkerHub.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        RoleManager<IdentityRole<Guid>> _roleManager;

        public UserService(UserManager<User> userManager, ITokenService jwtService, IMapper mapper, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _tokenService = jwtService;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        public async Task<UserDTO> RegisterUserAsync(RegisterUserDTO createModel, CancellationToken cancellationToken)
        {
            User user = _mapper.Map<User>(createModel);

            var result = await _userManager.CreateAsync(user, createModel.Password);

            if (!result.Succeeded)
            {
                throw new AppValidationException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await _userManager.AddToRoleAsync(user, AppRoles.User);

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<AuthenticationDTO> LoginUserAsync(LoginUserDTO loginModel)
        {
            var user = await _userManager.FindByEmailAsync(loginModel.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                throw new UnauthorizedException("Wrong password or email.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            return await _tokenService.GenerateJwt(user.Id, user.UserName, roles);
        }
        public async Task<AuthenticationDTO> RefreshTokensAsync(string oldRefreshToken)
        {
            var userId = await _tokenService.ValidateRefreshToken(oldRefreshToken);

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            return await _tokenService.GenerateJwt(user.Id, user.UserName, roles);
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

        public async Task AssignRoleAsync(AssignRoleDTO model, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
                throw new NotFoundException($"User with ID {model.UserId} not found.");

            if (!await _roleManager.RoleExistsAsync(model.RoleName))
                throw new AppValidationException($"role '{model.RoleName}' does not exist in the system.");

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            var result = await _userManager.AddToRoleAsync(user, model.RoleName);

            if (!result.Succeeded)
                throw new AppValidationException("Failed to assign role.");
        }
    }
}
