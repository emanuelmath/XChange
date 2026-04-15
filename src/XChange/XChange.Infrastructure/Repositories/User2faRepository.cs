using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using XChange.Core.Entities;
using XChange.Core.Interfaces;

namespace XChange.Infrastructure.Repositories
{
    public class User2faRepository(IDbConnectionFactory connectionFactory) : IUser2faRepository
    {
        public async Task<int> CreateAsync(User2fa user2fa)
        {
            using var conn = connectionFactory.CreateConnection();

            string storedProcedure = "sp_user_2fa_create";

            var parameters = new DynamicParameters();
            parameters.Add("@p_user_id", user2fa.UserId);
            parameters.Add("@psecret_key", user2fa.SecretKey);

            return await conn.QuerySingleAsync<int>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public Task Disable2faAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task Enable2faAsync(int userId, byte[] secretKey)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> GetSecretKeyAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool?> Is2faEnabledAsync(int userId)
        {
            using var conn = connectionFactory.CreateConnection();
            string query = "SELECT is_enabled AS IsEnabled FROM user_2fa WHERE user_id = @UserId";

            var result = await conn.QueryFirstOrDefaultAsync<bool?>(query, new { UserId = userId });
            return result ?? false;
        }
    }
}
