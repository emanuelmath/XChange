using XChange.Core.Constants;
using XChange.Core.Exceptions;

namespace XChange.Core.Entities
{
    public class User
    {
        public int Id { get; private set; }
        public string Email { get; private set; }
        public string? PasswordHash {  get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string AuthProvider { get; private set; }
        public string? GoogleId { get; private set; }
        public string Status { get; private set; } = UserStatus.Active;
        public bool IsEmailVerified { get; private set; } = false;
        public DateTime? CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        //Para el correcto uso con SqlClient y Dapper.
        private User() { }
        public User(
            string email, string firstName, 
            string lastName, string authProvider, 
            string? password = null, string? googleId = null)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new CreateUserException(nameof(Email));
            if (string.IsNullOrWhiteSpace(firstName)) throw new CreateUserException(nameof(FirstName));
            if (string.IsNullOrWhiteSpace(lastName)) throw new CreateUserException(nameof(LastName));
            if (string.IsNullOrWhiteSpace(authProvider)) throw new CreateUserException(nameof(AuthProvider));

            if (authProvider != Constants.AuthProvider.Local && authProvider != Constants.AuthProvider.Google)
            {
                throw new CreateUserException(nameof(AuthProvider));
            }

            if (password == null && googleId == null) throw new PasswordAndGoogleIdException();

                
            this.Email = email;
            this.PasswordHash = password;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.AuthProvider = authProvider;
            this.GoogleId = googleId;
        }
    }
}
