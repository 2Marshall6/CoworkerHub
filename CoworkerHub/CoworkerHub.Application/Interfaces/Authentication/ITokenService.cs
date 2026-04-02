using CoworkerHub.Application.DTOs.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoworkerHub.Application.Interfaces
{
    public interface ITokenService
    {
        Task<AuthenticationDTO> GenerateJwt(Guid userId, string userName);
        Task<Guid?> ValidateRefreshToken(string refreshToken);
        Task<string> GenerateRefreshToken(Guid userId);
    }
}
