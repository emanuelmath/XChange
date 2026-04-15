using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using XChange.Core.Entities;
using XChange.Core.Interfaces;

namespace XChange.Infrastructure.Repositories
{
    public class UserRepository(IDbConnectionFactory connectionFactory) : IUserRepository
    {
        public async Task<int> CreateAsync(User user)
        {
            using var conn = connectionFactory.CreateConnection();

            string storedProcedure = user.AuthProvider == "local"
                ? "sp_users_create_local"
                : "sp_users_create_google";

            var parameters = new DynamicParameters();
            parameters.Add("@p_email", user.Email);
            parameters.Add("@p_first_name", user.FirstName);
            parameters.Add("@p_last_name", user.LastName);

            if (user.AuthProvider == "local")
            {
                parameters.Add("@p_password_hash", user.PasswordHash);
            }
            else
            {
                parameters.Add("@p_google_id", user.GoogleId);
            }

            return await conn.QuerySingleAsync<int>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            using var conn = connectionFactory.CreateConnection();

            string query = @"
                    SELECT 
                       id, 
                       email, 
                       password_hash AS PasswordHash, 
                       first_name AS FirstName, 
                       last_name AS LastName, 
                       auth_provider AS AuthProvider, 
                       google_id AS GoogleId,
                       status,
                       is_email_verified AS IsEmailVerified
                       FROM users 
                    WHERE email = @Email";

            return await conn.QueryFirstOrDefaultAsync<User>(query, new { Email = email });
        }

        public async Task<int> LinkGoogleIdToUser(string googleId, int id)
        {
            using var conn = connectionFactory.CreateConnection();

            var sql = @"
                    UPDATE users
                    SET google_id = @googleId, is_email_verified = 1, auth_provider = 'google'
                    WHERE id = @id";

            return await conn.ExecuteAsync(sql, new
            {
                GoogleId = googleId,
                Id = id,
            });
        }

        public async Task<int> UpdateStatusAsync(int id, string status)
        {
            using var conn = connectionFactory.CreateConnection();

            var sql = @"
                    UPDATE users
                    SET user_status = @status
                    WHERE id = @id";

            return await conn.ExecuteAsync(sql, new
            {
                Status = status,
                Id = id,
            });
        }

        public async Task<int> VerifyEmailAsync(int id)
        {
            using var conn = connectionFactory.CreateConnection();

            var sql = @"
                    UPDATE users
                    SET is_email_verified = 1
                    WHERE id = @id";

            return await conn.ExecuteAsync(sql, new
            {
                Id = id,
            });
        }
    }
}
