using AutoMapper;
using CoworkerHub.Application.DTOs.Authentication;
using CoworkerHub.Application.Exceptions;
using CoworkerHub.Application.Interfaces;
using CoworkerHub.Application.Services;
using CoworkerHub.Domain.Entities;
using CoworkerHub.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace CoworkerHub.Tests.Unit.Services
{
    public class UserServiceTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<RoleManager<IdentityRole<Guid>>> _mockRoleManager;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserManager = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object,
                null, null, null, null, null, null, null, null
            );

            _mockRoleManager = new Mock<RoleManager<IdentityRole<Guid>>>(
                new Mock<IRoleStore<IdentityRole<Guid>>>().Object,
                null, null, null, null
            );

            _mockTokenService = new Mock<ITokenService>();
            _mockMapper = new Mock<IMapper>();

            _userService = new UserService(
                _mockUserManager.Object,
                _mockTokenService.Object,
                _mockMapper.Object,
                _mockRoleManager.Object
            );
        }

        #region RegisterUserAsync Tests

        [Fact]
        public async Task RegisterUserAsync_WithValidData_ReturnsUserDTO()
        {
            // Arrange
            var registerModel = new RegisterUserDTO
            {
                Email = "test@example.com",
                UserName = "testuser",
                Password = "Password123"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = registerModel.Email,
                UserName = registerModel.UserName
            };

            var userDTO = new UserDTO
            {
                Id = user.Id,
                Email = registerModel.Email,
                UserName = registerModel.UserName
            };

            _mockMapper.Setup(x => x.Map<User>(registerModel))
                .Returns(user);
            _mockUserManager.Setup(x => x.CreateAsync(user, registerModel.Password))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.AddToRoleAsync(user, AppRoles.User))
                .ReturnsAsync(IdentityResult.Success);
            _mockMapper.Setup(x => x.Map<UserDTO>(user))
                .Returns(userDTO);

            // Act
            var result = await _userService.RegisterUserAsync(registerModel, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(registerModel.Email, result.Email);
            Assert.Equal(registerModel.UserName, result.UserName);
            _mockUserManager.Verify(x => x.AddToRoleAsync(user, AppRoles.User), Times.Once);
        }

        [Fact]
        public async Task RegisterUserAsync_WhenCreateFails_ThrowsAppValidationException()
        {
            // Arrange
            var registerModel = new RegisterUserDTO
            {
                Email = "test@example.com",
                UserName = "testuser",
                Password = "Password123"
            };

            var user = new User();

            var identityError = new IdentityError { Description = "Password too weak" };
            var result = IdentityResult.Failed(identityError);

            _mockMapper.Setup(x => x.Map<User>(registerModel))
                .Returns(user);
            _mockUserManager.Setup(x => x.CreateAsync(user, registerModel.Password))
                .ReturnsAsync(result);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppValidationException>(
                () => _userService.RegisterUserAsync(registerModel, CancellationToken.None)
            );

            Assert.Contains("Password too weak", exception.Message);
        }

        #endregion

        #region LoginUserAsync Tests

        [Fact]
        public async Task LoginUserAsync_WithValidCredentials_ReturnsAuthenticationDTO()
        {
            // Arrange
            var loginModel = new LoginUserDTO
            {
                Email = "test@example.com",
                Password = "Password123"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = loginModel.Email,
                UserName = "testuser"
            };

            var roles = new List<string> { AppRoles.User };

            var authDTO = new AuthenticationDTO
            {
                UserName = user.UserName,
                AccessToken = "jwt_token",
                ExpiresIn = 3600,
                RefreshToken = "refresh_token"
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(loginModel.Email))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.CheckPasswordAsync(user, loginModel.Password))
                .ReturnsAsync(true);
            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(roles);
            _mockTokenService.Setup(x => x.GenerateJwt(user.Id, user.UserName, roles))
                .ReturnsAsync(authDTO);

            // Act
            var result = await _userService.LoginUserAsync(loginModel);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(authDTO.AccessToken, result.AccessToken);
            Assert.Equal(authDTO.RefreshToken, result.RefreshToken);
        }

        [Fact]
        public async Task LoginUserAsync_WithNonExistentEmail_ThrowsUnauthorizedException()
        {
            // Arrange
            var loginModel = new LoginUserDTO
            {
                Email = "nonexistent@example.com",
                Password = "Password123"
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(loginModel.Email))
                .ReturnsAsync((User)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedException>(
                () => _userService.LoginUserAsync(loginModel)
            );

            Assert.Contains("Wrong password or email", exception.Message);
        }

        [Fact]
        public async Task LoginUserAsync_WithWrongPassword_ThrowsUnauthorizedException()
        {
            // Arrange
            var loginModel = new LoginUserDTO
            {
                Email = "test@example.com",
                Password = "WrongPassword"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = loginModel.Email
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(loginModel.Email))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.CheckPasswordAsync(user, loginModel.Password))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedException>(
                () => _userService.LoginUserAsync(loginModel)
            );

            Assert.Contains("Wrong password or email", exception.Message);
        }

        #endregion

        #region RefreshTokensAsync Tests

        [Fact]
        public async Task RefreshTokensAsync_WithValidToken_ReturnsNewAuthenticationDTO()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var oldRefreshToken = "old_refresh_token";

            var user = new User
            {
                Id = userId,
                UserName = "testuser"
            };

            var roles = new List<string> { AppRoles.User };

            var authDTO = new AuthenticationDTO
            {
                UserName = user.UserName,
                AccessToken = "new_jwt_token",
                ExpiresIn = 3600,
                RefreshToken = "new_refresh_token"
            };

            _mockTokenService.Setup(x => x.ValidateRefreshToken(oldRefreshToken))
                .ReturnsAsync(userId);
            _mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(roles);
            _mockTokenService.Setup(x => x.GenerateJwt(user.Id, user.UserName, roles))
                .ReturnsAsync(authDTO);

            // Act
            var result = await _userService.RefreshTokensAsync(oldRefreshToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(authDTO.AccessToken, result.AccessToken);
            Assert.NotEqual(oldRefreshToken, result.RefreshToken);
        }

        [Fact]
        public async Task RefreshTokensAsync_WithInvalidToken_ThrowsUnauthorizedException()
        {
            // Arrange
            var invalidToken = "invalid_refresh_token";

            _mockTokenService.Setup(x => x.ValidateRefreshToken(invalidToken))
                .ThrowsAsync(new UnauthorizedException("Refresh token is expired"));

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(
                () => _userService.RefreshTokensAsync(invalidToken)
            );
        }

        [Fact]
        public async Task RefreshTokensAsync_WithNonExistentUser_ThrowsNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var refreshToken = "refresh_token";

            _mockTokenService.Setup(x => x.ValidateRefreshToken(refreshToken))
                .ReturnsAsync(userId);
            _mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((User)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _userService.RefreshTokensAsync(refreshToken)
            );

            Assert.Contains("User not found", exception.Message);
        }

        #endregion

        #region ChangePasswordAsync Tests

        [Fact]
        public async Task ChangePasswordAsync_WithValidData_ChangesPassword()
        {
            // Arrange
            var changeModel = new ChangeUserPasswordDTO
            {
                Email = "test@example.com",
                CurrentPassword = "OldPassword123",
                NewPassword = "NewPassword123"
            };

            var user = new User { Email = changeModel.Email };

            _mockUserManager.Setup(x => x.FindByEmailAsync(changeModel.Email))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.ChangePasswordAsync(user, changeModel.CurrentPassword, changeModel.NewPassword))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _userService.ChangePasswordAsync(changeModel, CancellationToken.None);

            // Assert
            _mockUserManager.Verify(
                x => x.ChangePasswordAsync(user, changeModel.CurrentPassword, changeModel.NewPassword),
                Times.Once
            );
        }

        [Fact]
        public async Task ChangePasswordAsync_WithWrongCurrentPassword_ThrowsAppValidationException()
        {
            // Arrange
            var changeModel = new ChangeUserPasswordDTO
            {
                Email = "test@example.com",
                CurrentPassword = "WrongPassword",
                NewPassword = "NewPassword123"
            };

            var user = new User { Email = changeModel.Email };

            var identityError = new IdentityError { Description = "Incorrect password" };
            var result = IdentityResult.Failed(identityError);

            _mockUserManager.Setup(x => x.FindByEmailAsync(changeModel.Email))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.ChangePasswordAsync(user, changeModel.CurrentPassword, changeModel.NewPassword))
                .ReturnsAsync(result);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppValidationException>(
                () => _userService.ChangePasswordAsync(changeModel, CancellationToken.None)
            );

            Assert.Contains("Incorrect password", exception.Message);
        }

        #endregion

        #region AssignRoleAsync Tests

        [Fact]
        public async Task AssignRoleAsync_WithValidData_AssignsRole()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var assignModel = new AssignRoleDTO
            {
                UserId = userId,
                RoleName = AppRoles.Manager
            };

            var user = new User { Id = userId };
            var currentRoles = new List<string> { AppRoles.User };

            _mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);
            _mockRoleManager.Setup(x => x.RoleExistsAsync(AppRoles.Manager))
                .ReturnsAsync(true);
            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(currentRoles);
            _mockUserManager.Setup(x => x.RemoveFromRolesAsync(user, currentRoles))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.AddToRoleAsync(user, AppRoles.Manager))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _userService.AssignRoleAsync(assignModel, CancellationToken.None);

            // Assert
            _mockUserManager.Verify(x => x.RemoveFromRolesAsync(user, currentRoles), Times.Once);
            _mockUserManager.Verify(x => x.AddToRoleAsync(user, AppRoles.Manager), Times.Once);
        }

        [Fact]
        public async Task AssignRoleAsync_WithNonExistentUser_ThrowsNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var assignModel = new AssignRoleDTO
            {
                UserId = userId,
                RoleName = AppRoles.Manager
            };

            _mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((User)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _userService.AssignRoleAsync(assignModel, CancellationToken.None)
            );

            Assert.Contains($"User with ID {userId} not found", exception.Message);
        }

        [Fact]
        public async Task AssignRoleAsync_WithNonExistentRole_ThrowsAppValidationException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var assignModel = new AssignRoleDTO
            {
                UserId = userId,
                RoleName = "NonExistentRole"
            };

            var user = new User { Id = userId };

            _mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);
            _mockRoleManager.Setup(x => x.RoleExistsAsync("NonExistentRole"))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppValidationException>(
                () => _userService.AssignRoleAsync(assignModel, CancellationToken.None)
            );

            Assert.Contains("does not exist in the system", exception.Message);
        }

        #endregion
    }
}
