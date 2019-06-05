using System;
using System.Collections.Generic;
using System.Text;

namespace core.Domain.Entities.Auth
{
    public class TokenResponse
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public int Expiration { get; set; }
    }
}
