using System.Security.Cryptography;
using System.Text;

namespace ProdInfoSys.Classes
{
    /// <summary>
    /// Provides methods for encrypting and decrypting data using the Windows Data Protection API (DPAPI) with the
    /// current user's data protection scope.
    /// </summary>
    /// <remarks>Use this class to securely store sensitive information that should only be accessible to the
    /// current user on the same machine. The encrypted data cannot be decrypted by other users or on different
    /// machines. This class is suitable for protecting application secrets, credentials, or other confidential data in
    /// user-specific scenarios.</remarks>
    public static class DpApiStorage
    {
        /// <summary>
        /// Encrypts the specified plain text using the current user's data protection scope.
        /// </summary>
        /// <remarks>The encrypted data can only be decrypted by the same user account on the same
        /// machine. Use this method to securely store sensitive information that should be accessible only to the
        /// current user.</remarks>
        /// <param name="plainText">The plain text string to encrypt. Cannot be null.</param>
        /// <returns>A Base64-encoded string containing the encrypted data.</returns>
        public static string Encrypt(string plainText)
        {
            byte[] data = Encoding.UTF8.GetBytes(plainText);
            byte[] encrypted = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encrypted);
        }

        /// <summary>
        /// Decrypts the specified Base64-encoded, user-protected string and returns the original plain text.
        /// </summary>
        /// <remarks>This method uses the Windows Data Protection API (DPAPI) with the current user's
        /// scope. The method will only successfully decrypt strings that were encrypted under the same user account. If
        /// the input is not a valid Base64-encoded, user-protected string, an exception may be thrown.</remarks>
        /// <param name="cipherText">The Base64-encoded string to decrypt. This value must have been encrypted using the current user's data
        /// protection scope.</param>
        /// <returns>The decrypted plain text string.</returns>
        public static string Decrypt(string cipherText)
        {
            byte[] data = Convert.FromBase64String(cipherText);
            byte[] decrypted = ProtectedData.Unprotect(data, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
