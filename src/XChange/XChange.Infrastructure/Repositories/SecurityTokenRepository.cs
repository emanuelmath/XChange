using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using XChange.Core.Entities;
using XChange.Core.Interfaces;

namespace XChange.Infrastructure.Repositories
{
    public class SecurityTokenRepository(IDbConnectionFactory connectionFactory) : ISecurityTokenRepository
    {
        public async Task ChangeToUsedAsync(int tokenId)
        {
            using var conn = connectionFactory.CreateConnection();

            string sql = @"
                UPDATE security_tokens 
                SET used = 1 
                WHERE id = @Id";

            await conn.ExecuteAsync(sql, new { Id = tokenId });
        }

        public async Task<int> CreateAsync(SecurityToken securityToken)
        {
            using var conn = connectionFactory.CreateConnection();

            string storedProcedure = "sp_security_tokens_create";

            var parameters = new DynamicParameters();
            parameters.Add("@p_user_id", securityToken.UserId);
            parameters.Add("@p_token_hash", securityToken.TokenHash);
            parameters.Add("@p_type", securityToken.Type);
            parameters.Add("@p_expires_at", securityToken.ExpiresAt);
            parameters.Add("@p_used", securityToken.Used);
            parameters.Add("@p_created_at", securityToken.CreatedAt);

            return await conn.QuerySingleAsync<int>(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<SecurityToken?> GetValidTokenAsync(int userId, string type)
        {
            using var conn = connectionFactory.CreateConnection();

            string sql = @"
                SELECT TOP 1 
                    id AS Id, 
                    user_id AS UserId, 
                    token_hash AS TokenHash, 
                    type AS Type, 
                    expires_at AS ExpiresAt, 
                    used AS Used, 
                    created_at AS CreatedAt
                FROM security_tokens
                WHERE user_id = @UserId 
                  AND type = @Type 
                  AND used = 0 
                  AND expires_at > SYSUTCDATETIME()
                ORDER BY created_at DESC";

            return await conn.QueryFirstOrDefaultAsync<SecurityToken>(sql, new { UserId = userId, Type = type });
        }

        public async Task DeleteExpiredTokensAsync()
        {
            using var conn = connectionFactory.CreateConnection();
            string sql = "DELETE FROM security_tokens WHERE expires_at < SYSUTCDATETIME() OR used = 1";
            await conn.ExecuteAsync(sql);
        }
    }
}
