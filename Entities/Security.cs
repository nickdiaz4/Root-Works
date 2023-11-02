using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace Catawba.Entities
{
    public static class Security
    {
        // List of characters to generat random unique passwords with
        internal static readonly char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!?@#$%&".ToCharArray();

        /// <summary>
        /// Creates a unique 8-character long key securely generating a random
        /// assortment of characters found in the char array "chars[]" above.
        /// </summary>
        /// 
        /// <returns>
        /// A randomly generated password for a new account
        /// or for resseting an existing account's password. 
        ///</returns>
        public static string createUniqueKey()
        {
            // 8 = size of the key being made
            byte[] data = new byte[4 * 8];
            using (var crypto = RandomNumberGenerator.Create())
            {
                crypto.GetBytes(data);
            }

            StringBuilder uniqueKey = new StringBuilder(8);
            for (int i = 0; i < 8; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % chars.Length;

                uniqueKey.Append(chars[idx]);
            }
            return uniqueKey.ToString();
        }

        public static string HashPassword(string password)
        {

            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                // fill salt array with random bytes
                rng.GetBytes(salt);
            }

            // compute hash with salt
            byte[] hashedPassword = HashPasswordWithSalt(password, salt);

            // return both concatenated as strings
            return Convert.ToBase64String(salt) + Convert.ToBase64String(hashedPassword);
        }

        private static byte[] HashPasswordWithSalt(string password, byte[] salt)
        {
            using (var sha256 = SHA256.Create())
            {
                // convert the password and salt to byte array
                byte[] saltedPassword = System.Text.Encoding.UTF8.GetBytes(password + Convert.ToBase64String(salt));

                // return byte array of the computed hash
                return sha256.ComputeHash(saltedPassword);
            }
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // parse out the salt and hash of the stored password 
            // salt is fixed length from (0,24)
            byte[] salt = Convert.FromBase64String(hashedPassword.Substring(0, 24));
            byte[] storedHash = Convert.FromBase64String(hashedPassword.Substring(24));

            // hash the new password with the same salt of the stored saved
            byte[] hash = HashPasswordWithSalt(password, salt);

            // check stored hash with new hash for equality
            return storedHash.SequenceEqual(hash);
        }
    }
}
