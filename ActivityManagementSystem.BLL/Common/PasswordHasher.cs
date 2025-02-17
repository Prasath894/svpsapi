namespace ActivityManagementSystem.BLL.Common
{
    /// <summary>
    /// Password hasher class
    /// </summary>
    public class PasswordHasher
    {
        /// <summary>
        /// Hashes a password using bcrypt.
        /// </summary>
        /// <param name="password">The password to be hashed.</param>
        /// <returns>Hashed password.</returns>
        public static string HashPassword(string password)
        {
            // Generate a salt and hash the password with it
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

        /// <summary>
        /// Verifies a password against a hashed password.
        /// </summary>
        /// <param name="password">The password to be verified.</param>
        /// <param name="hashedPassword">The hashed password that to be verified.</param>
        /// <returns>Verification status of password and hashed password.</returns>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
