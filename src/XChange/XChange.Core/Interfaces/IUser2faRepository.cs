using System;
using System.Collections.Generic;
using System.Text;
using XChange.Core.Entities;

namespace XChange.Core.Interfaces
{
    public interface IUser2faRepository
    {
            //Task<int> CreateAsync(User2fa user2fa);
            Task<bool?> Is2faEnabledAsync(int userId);
            //Task<byte[]> GetSecretKeyAsync(int userId); 
           // Task Enable2faAsync(int userId, byte[] secretKey);
          //  Task Disable2faAsync(int userId);
    }
}
