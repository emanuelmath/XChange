using System;
using System.Collections.Generic;
using System.Text;
using XChange.Core.Exceptions;

namespace XChange.Core.Entities
{
    public class User2fa
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public byte[] SecretKey { get; private set; }
        public bool IsEnabled { get; private set; } = true;
        public DateTime? CreatedAt { get; private set; }

        public User2fa(int userId, byte[] secretKey)
        {
            if (userId <= 0) throw new CreateUser2faException(nameof(UserId));
            if (secretKey == null || secretKey.Length <= 0) throw new CreateUser2faException(nameof(SecretKey));

            UserId = userId;
            SecretKey = secretKey;
        }
    }
}
