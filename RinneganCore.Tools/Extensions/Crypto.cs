using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RinneganCore.Tools.Extensions
{
    public static class Crypto
    {
        /// <summary>
        /// Hash a string with static algorythm
        /// </summary>
        /// <returns>hashed string result</returns>
        public static string Hash(this string @this)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(@this));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }


        /// <summary>
        /// Encrypt a string with static algorythm and given key
        /// </summary>
        /// <param name="this">plain Text</param>
        /// <param name="key">The key should be of a size 128, 192, or 256 bits</param>
        /// <returns>encrypted string result</returns>
        public static string Encrypt(this string @this, string key)
        {
            _Crypto _crypto = new _Crypto(key);
            return _crypto.Encrypt(@this);
        }


        /// <summary>
        /// Decrypt a string with static algorythm and given key
        /// </summary>
        /// <param name="this">cipher Text Base64</param>
        /// <param name="key">The key should be of a size 128, 192, or 256 bits</param>
        /// <returns>resolved string result</returns>
        public static string Decrypt(this string @this, string key)
        {
            _Crypto _crypto = new _Crypto(key);
            return _crypto.Decrypt(@this);
        }



        /// <summary>
        /// Create a random string with specified length
        /// </summary>
        /// <param name="length">result string length</param>
        /// <param name="allowedChars"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">minimum length is 1</exception>
        /// <exception cref="ArgumentException">allowed characters can not be empty</exception>
        public static string RandomString(int length, string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
        {
            if (length < 1) throw new ArgumentOutOfRangeException("length", "length cannot be less than one.");
            if (string.IsNullOrEmpty(allowedChars)) throw new ArgumentException("allowedChars may not be empty.");

            const int byteSize = 0x100;
            var allowedCharSet = new HashSet<char>(allowedChars).ToArray();
            if (byteSize < allowedCharSet.Length) throw new ArgumentException(string.Format("allowedChars may contain no more than {0} characters.", byteSize));

            // Guid.NewGuid and System.Random are not particularly random. By using a
            // cryptographically-secure random number generator, the caller is always
            // protected, regardless of use.
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                var result = new StringBuilder();
                var buf = new byte[128];
                while (result.Length < length)
                {
                    rng.GetBytes(buf);
                    for (var i = 0; i < buf.Length && result.Length < length; ++i)
                    {
                        // Divide the byte into allowedCharSet-sized groups. If the
                        // random value falls into the last group and the last group is
                        // too small to choose from the entire allowedCharSet, ignore
                        // the value in order to avoid biasing the result.
                        var outOfRangeStart = byteSize - byteSize % allowedCharSet.Length;
                        if (outOfRangeStart <= buf[i]) continue;
                        result.Append(allowedCharSet[buf[i] % allowedCharSet.Length]);
                    }
                }
                //bool addControlChar = false;
                //if (addControlChar)
                //{
                //    int sum = 0;
                //    for (int i = 0; i < result.Length; i++)
                //    {
                //        sum += result[i] * (i + 1);
                //    }
                //    return $"{result}{allowedChars[sum % allowedChars.Length]}";
                //}
                return result.ToString();
            }
        }
    }

    internal class _Crypto
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public _Crypto(string key)
        {
            // The key should be of a size 128, 192, or 256 bits.
            _key = Encoding.UTF8.GetBytes(key);
            // AES block size is fixed at 128 bits, so the IV should be of 128 bits as well.
            _iv = new byte[16]; // Initialization vector.
        }

        public string Encrypt(string plainText)
        {
            using (var aesAlg = Aes.Create())
            {
                using (var encryptor = aesAlg.CreateEncryptor(_key, _iv))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        var encrypted = msEncrypt.ToArray();
                        return Convert.ToBase64String(encrypted);
                    }
                }
            }
        }

        public string Decrypt(string cipherTextBase64)
        {
            var cipherText = Convert.FromBase64String(cipherTextBase64);
            using (var aesAlg = Aes.Create())
            {
                using (var decryptor = aesAlg.CreateDecryptor(_key, _iv))
                {
                    using (var msDecrypt = new MemoryStream(cipherText))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

    }
}
