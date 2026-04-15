using System;
using System.Collections.Generic;
using System.Text;
using XChange.Core.Entities;

namespace XChange.Core.Interfaces
{
    public interface ISecurityTokenRepository
    {
        Task<int> CreateAsync(SecurityToken securityToken);
        Task ChangeToUsedAsync(int tokenId);
        Task<SecurityToken?> GetValidTokenAsync(int userId, string type);
        Task DeleteExpiredTokensAsync();
    }
}
