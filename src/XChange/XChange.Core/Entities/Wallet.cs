using System;
using System.Collections.Generic;
using System.Text;
using XChange.Core.Exceptions;

namespace XChange.Core.Entities
{
    public class Wallet
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public decimal Balance { get; private set; } = 0.0m;
        public string Currency { get; private set; } = "USD";
        public DateTime? CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public Wallet(int userId, decimal balance)
        {
            if (userId <= 0) throw new CreateWalletException(nameof(UserId));
            if (balance < 0) throw new CreateWalletException(nameof(Balance));

            UserId = userId;
            Balance = balance;
        }
    }
}
