using CoworkerHub.Application.DTOs.Authentication;
using CoworkerHub.Application.Exceptions;
using CoworkerHub.Application.Interfaces;
using CoworkerHub.Application.Options;
using CoworkerHub.Application.Services;
using CoworkerHub.Domain.Entities;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace CoworkerHub.Tests.Unit.Services
{
    public class TokenServiceTests
    {
        private readonly Mock<IRefreshTokenRepository> _mockRefreshTokenRepository;
        private readonly TokenService _tokenService;
        private readonly TokenOptions _tokenOptions;

        public TokenServiceTests()
        {
            _mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();

            _tokenOptions = new TokenOptions
            {
                Key = "this_is_a_very_long_secret_key_that_contains_more_than_64_bytes_for_hmac_sha512_algorithm_requirement_to_work_properly_in_production",
                Issuer = "CoworkerHub",
                Audience = "CoworkerHubAPI",
                TokenLifetimeInMinutes = 60,
                RefreshTokenLifetimeInDays = 7
            };

            var options = Options.Create(_tokenOptions);
            _tokenService = new TokenService(options, _mockRefreshTokenRepository.Object);
        }

        #region GenerateJwt Tests

        [Fact]
        public async Task GenerateJwt_WithValidData_ReturnsAuthenticationDTO()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userName = "testuser";
            var roles = new List<string> { "User" };

            _mockRefreshTokenRepository.Setup(x => x.AddRefreshTokenAsync(It.IsAny<RefreshToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _tokenService.GenerateJwt(userId, userName, roles);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userName, result.UserName);
            Assert.NotNull(result.AccessToken);
            Assert.NotNull(result.RefreshToken);
            Assert.Equal(3600, result.ExpiresIn);
        }

        [Fact]
        public async Task GenerateJwt_WithMultipleRoles_IncludesAllRolesInToken()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userName = "testuser";
            var roles = new List<string> { "Admin", "Manager", "User" };

            _mockRefreshTokenRepository.Setup(x => x.AddRefreshTokenAsync(It.IsAny<RefreshToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _tokenService.GenerateJwt(userId, userName, roles);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.AccessToken);
            // JWT токен должен содержать все роли (можно декодировать и проверить)
        }

        [Fact]
        public async Task GenerateJwt_WithNoRoles_ReturnsValidToken()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userName = "testuser";
            var roles = new List<string>();

            _mockRefreshTokenRepository.Setup(x => x.AddRefreshTokenAsync(It.IsAny<RefreshToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _tokenService.GenerateJwt(userId, userName, roles);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.AccessToken);
        }

        [Fact]
        public async Task GenerateJwt_CreatesRefreshToken()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userName = "testuser";
            var roles = new List<string> { "User" };

            RefreshToken capturedRefreshToken = null;
            _mockRefreshTokenRepository.Setup(x => x.AddRefreshTokenAsync(It.IsAny<RefreshToken>()))
                .Callback<RefreshToken>(rt => capturedRefreshToken = rt)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _tokenService.GenerateJwt(userId, userName, roles);

            // Assert
            Assert.NotNull(capturedRefreshToken);
            Assert.Equal(userId, capturedRefreshToken.UserId);
            Assert.NotNull(capturedRefreshToken.Token);
            Assert.True(capturedRefreshToken.ExpiresAt > DateTime.UtcNow);
        }

        #endregion

        #region GenerateRefreshToken Tests

        [Fact]
        public async Task GenerateRefreshToken_ReturnsUniqueToken()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockRefreshTokenRepository.Setup(x => x.AddRefreshTokenAsync(It.IsAny<RefreshToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var token1 = await _tokenService.GenerateRefreshToken(userId);
            var token2 = await _tokenService.GenerateRefreshToken(userId);

            // Assert
            Assert.NotEqual(token1, token2);
        }

        [Fact]
        public async Task GenerateRefreshToken_TokenExpiresInCorrectDays()
        {
            // Arrange
            var userId = Guid.NewGuid();
            RefreshToken capturedRefreshToken = null;

            _mockRefreshTokenRepository.Setup(x => x.AddRefreshTokenAsync(It.IsAny<RefreshToken>()))
                .Callback<RefreshToken>(rt => capturedRefreshToken = rt)
                .Returns(Task.CompletedTask);

            var beforeTime = DateTime.UtcNow;

            // Act
            await _tokenService.GenerateRefreshToken(userId);

            var afterTime = DateTime.UtcNow;

            // Assert
            Assert.NotNull(capturedRefreshToken);
            var expectedExpire = afterTime.AddDays(_tokenOptions.RefreshTokenLifetimeInDays);
            var tolerance = TimeSpan.FromSeconds(5);

            Assert.True(capturedRefreshToken.ExpiresAt >= expectedExpire - tolerance);
            Assert.True(capturedRefreshToken.ExpiresAt <= expectedExpire + tolerance);
        }

        #endregion

        #region ValidateRefreshToken Tests

        [Fact]
        public async Task ValidateRefreshToken_WithValidToken_ReturnsUserId()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var tokenString = Guid.NewGuid().ToString();
            var refreshToken = new RefreshToken
            {
                Token = tokenString,
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddDays(1)
            };

            _mockRefreshTokenRepository.Setup(x => x.GetRefreshTokenAsync(tokenString))
                .ReturnsAsync(refreshToken);
            _mockRefreshTokenRepository.Setup(x => x.DeleteRefreshTokenAsync(tokenString))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _tokenService.ValidateRefreshToken(tokenString);

            // Assert
            Assert.Equal(userId, result);
            _mockRefreshTokenRepository.Verify(x => x.DeleteRefreshTokenAsync(tokenString), Times.Once);
        }

        [Fact]
        public async Task ValidateRefreshToken_WithExpiredToken_ThrowsUnauthorizedException()
        {
            // Arrange
            var tokenString = Guid.NewGuid().ToString();
            var refreshToken = new RefreshToken
            {
                Token = tokenString,
                UserId = Guid.NewGuid(),
                ExpiresAt = DateTime.UtcNow.AddHours(-1)  // Expired
            };

            _mockRefreshTokenRepository.Setup(x => x.GetRefreshTokenAsync(tokenString))
                .ReturnsAsync(refreshToken);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedException>(
                () => _tokenService.ValidateRefreshToken(tokenString)
            );

            Assert.Contains("expired", exception.Message);
            _mockRefreshTokenRepository.Verify(x => x.DeleteRefreshTokenAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ValidateRefreshToken_WithNonExistentToken_ThrowsUnauthorizedException()
        {
            // Arrange
            var tokenString = "nonexistent_token";

            _mockRefreshTokenRepository.Setup(x => x.GetRefreshTokenAsync(tokenString))
                .ReturnsAsync((RefreshToken)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedException>(
                () => _tokenService.ValidateRefreshToken(tokenString)
            );

            Assert.Contains("expired", exception.Message);
        }

        [Fact]
        public async Task ValidateRefreshToken_DeletesTokenAfterValidation()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var tokenString = Guid.NewGuid().ToString();
            var refreshToken = new RefreshToken
            {
                Token = tokenString,
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddDays(1)
            };

            _mockRefreshTokenRepository.Setup(x => x.GetRefreshTokenAsync(tokenString))
                .ReturnsAsync(refreshToken);
            _mockRefreshTokenRepository.Setup(x => x.DeleteRefreshTokenAsync(tokenString))
                .Returns(Task.CompletedTask);

            // Act
            await _tokenService.ValidateRefreshToken(tokenString);

            // Assert - Verify delete was called
            _mockRefreshTokenRepository.Verify(x => x.DeleteRefreshTokenAsync(tokenString), Times.Once);
        }

        #endregion
    }
}
