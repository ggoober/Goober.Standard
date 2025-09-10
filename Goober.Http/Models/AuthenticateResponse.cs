using System;

namespace Goober.Http.Models
{
    public class AuthenticateResponse
    {
        public string Token { get; set; }

        public DateTime? ExpireAt { get; set; }

        public string TokenType { get; set; }
    }
}
