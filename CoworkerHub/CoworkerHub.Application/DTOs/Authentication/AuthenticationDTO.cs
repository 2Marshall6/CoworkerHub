using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CoworkerHub.Application.DTOs.Authentication
{
    public class AuthenticationDTO
    {
        public string UserName { get; set; }
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        [JsonIgnore]
        public string RefreshToken { get; set; }
    }
}
