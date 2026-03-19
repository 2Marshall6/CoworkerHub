using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoworkerHub.Application.DTOs.Authentication
{
    public class LoginUserDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
