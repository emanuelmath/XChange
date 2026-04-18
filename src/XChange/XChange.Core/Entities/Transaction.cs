using System;
using System.Collections.Generic;
using System.Text;
using XChange.Core.Constants;
using XChange.Core.Exceptions;

namespace XChange.Core.Entities
{
    public class Transaction
    {
        public int Id { get; private set; }
        public int WalletId { get; private set; }
        public string Type { get; private set; } 
        public decimal Amount { get; private set; }
        public string Status { get; private set; } = TransactionStatus.Pending;
        public string Reference { get; private set; }
        public DateTime? CreatedAt { get; private set; }

        public Transaction(int walletId, string type, decimal amount, string reference)
        {
            if(walletId <= 0) throw new CreateTransactionException(nameof(WalletId));
            if(amount <= 0) throw new CreateTransactionException(nameof(Amount));
            if(string.IsNullOrWhiteSpace(type)) throw new CreateTransactionException(nameof(Type));
            if(type != TransactionType.Transfer || type != TransactionType.Deposit || type != TransactionType.Withdraw) throw new CreateTransactionException(nameof(Type));
            if(!string.IsNullOrWhiteSpace(reference)) throw new CreateTransactionException(nameof(Reference));

            WalletId = walletId;
            Type = type;
            Amount = amount;
            Reference = reference;
        }
    }
}
