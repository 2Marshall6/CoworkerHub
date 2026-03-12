using CoworkerHub.Application.DTOs;
using CoworkerHub.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CoworkerHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _authorizationService;
        private readonly IValidator<RegisterUserDTO> _registerValidator;
        private readonly IValidator<LoginUserDTO> _loginValidator;

        public UserController(IUserService workspaceService, IValidator<RegisterUserDTO> registerValidator, IValidator<LoginUserDTO> loginValodator, ITokenService authorizationService)
        {
            _userService = workspaceService;
            _registerValidator = registerValidator;
            _loginValidator = loginValodator;
            _authorizationService = authorizationService;
        }

        [HttpPost]
        public async Task<ActionResult<UserDTO>> Create(RegisterUserDTO createModel, CancellationToken cancellationToken)
        {
            var validationResult = await _registerValidator.ValidateAsync(createModel, cancellationToken);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var user = await _userService.RegisterUserAsync(createModel, cancellationToken);

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthenticationDTO>> Login(LoginUserDTO loginModel)
        {
            var validationResult = await _loginValidator.ValidateAsync(loginModel);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var authResult = await _userService.LoginUserAsync(loginModel);

            Response.Cookies.Append("refreshToken", authResult.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Включишь, когда будет HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(authResult);
        }
        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<ActionResult<AuthenticationDTO>> Refresh()
        {
            var oldToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrWhiteSpace(oldToken))
            {
                return Unauthorized("No refresh token found in cookies.");
            }

            var result = await _userService.RefreshTokensAsync(oldToken);

            if (result == null)
            {
                return Unauthorized("Invalid or expired refresh token.");
            }

            // 3. Кладем НОВЫЙ рефреш-токен обратно в куку
            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7) // Или сколько у тебя живет токен
            });

            // 4. Отдаем на фронтенд ТОЛЬКО Access токен
            // (RefreshToken из DTO лучше вообще убрать, чтобы фронтенд его даже не видел)
            return Ok(new { result.AccessToken, result.UserName, result.ExpiresIn });
        }
    }
}
