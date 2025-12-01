using System.Text;

namespace Core.Utilities.Security.Hashing
{
    public static class HashingHelper
    {
        /// <summary>
        /// Creates password hash and salt using HMACSHA512.
        /// </summary>
        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        /// <summary>
        /// Verifies password against stored hash and salt.
        /// </summary>
        public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            if (computedHash.Length != passwordHash.Length)
            {
                return false;
            }

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != passwordHash[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
