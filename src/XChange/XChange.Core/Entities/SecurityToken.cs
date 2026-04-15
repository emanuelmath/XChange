using System;
using System.Collections.Generic;
using System.Text;
using XChange.Core.Exceptions;
using XChange.Core.Constants;

namespace XChange.Core.Entities
{
    public class SecurityToken
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public string TokenHash { get; private set; }
        public string Type { get; private set; } 
        public DateTime ExpiresAt { get; private set; }
        public bool Used { get; private set; }
        public DateTime CreatedAt { get; private set; }


        private SecurityToken() { }

        public SecurityToken(int userId, string tokenHash, string type, int expirationMinutes = 15)
        {
            if (userId <= 0) throw new CreateSecurityTokenException(nameof(UserId));
            if (string.IsNullOrWhiteSpace(tokenHash)) throw new CreateSecurityTokenException(nameof(TokenHash));
            if (string.IsNullOrWhiteSpace(type))
                throw new CreateSecurityTokenException(nameof(Type));

            UserId = userId;
            TokenHash = tokenHash;
            Type = type;
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);
            Used = false;
            CreatedAt = DateTime.UtcNow;
        }

    }
}
