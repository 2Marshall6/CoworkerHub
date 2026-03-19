using CoworkerHub.Application.DTOs.Authentication;
using CoworkerHub.Application.Exceptions;
using CoworkerHub.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkerHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IValidator<RegisterUserDTO> _registerValidator;
        private readonly IValidator<LoginUserDTO> _loginValidator;

        public UserController(IUserService userService, IValidator<RegisterUserDTO> registerValidator, IValidator<LoginUserDTO> loginValidator)
        {
            _userService = userService;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<UserDTO>> Create(RegisterUserDTO createModel, CancellationToken cancellationToken)
        {
            var validationResult = await _registerValidator.ValidateAsync(createModel, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new AppValidationException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var user = await _userService.RegisterUserAsync(createModel, cancellationToken);

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthenticationDTO>> Login(LoginUserDTO loginModel)
        {
            var validationResult = await _loginValidator.ValidateAsync(loginModel);
            if (!validationResult.IsValid)
            {
                throw new AppValidationException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
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
                throw new UnauthorizedException("Refresh token is missing.");
            }

            var result = await _userService.RefreshTokensAsync(oldToken);

            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(result);
        }
    }
}
