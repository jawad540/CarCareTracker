using CarCareTracker.Models;
using CarCareTracker.Models.ViewModels;
using CarCareTracker.Repositories;

namespace CarCareTracker.Services
{
    /// <summary>
    /// Business Logic Layer for authentication.
    /// Uses BCrypt for password hashing and JwtService for tokens.
    /// </summary>
    public class AuthService
    {
        private readonly UserRepository _users = new UserRepository();
        private readonly JwtService _jwt = new JwtService();

        public const int ROLE_ADMIN = 1;
        public const int ROLE_USER = 2;

        /// <summary>Returns JWT token on success, null on failure.</summary>
        public string Login(LoginViewModel model, out User user)
        {
            user = _users.GetByEmail(model.Email);
            if (user == null) return null;
            if (!user.IsActive) return null;
            if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash)) return null;

            return _jwt.GenerateToken(user);
        }

        /// <summary>Registers a new user (role = User). Returns error message or null on success.</summary>
        public string Register(RegisterViewModel model)
        {
            if (_users.EmailExists(model.Email))
                return "An account with this email already exists.";

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                RoleId = ROLE_USER
            };

            _users.Create(user);
            return null;
        }
    }
}
