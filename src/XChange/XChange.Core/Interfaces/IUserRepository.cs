using System;
using System.Collections.Generic;
using System.Text;
using XChange.Core.Entities;

namespace XChange.Core.Interfaces
{
    public interface IUserRepository
    {
        //Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        //Task<User?> GetByGoogleIdAsync(string googleId);
        Task<int> CreateAsync(User user);
        Task<int> LinkGoogleIdToUser(string googleId, int id);
        //Task UpdatePasswordAsync(int userId, string newPasswordHash);
        Task<int> UpdateStatusAsync(int id, string status);
        Task<int> VerifyEmailAsync(int id);
    }
}
