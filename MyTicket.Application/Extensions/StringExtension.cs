using System.Security.Cryptography;
using System.Text;

namespace MyTicket.Application.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// Create new hash string using SHA256
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToSHA256(this string s)
        {
            using (SHA256 shaManager = new SHA256Managed())
            {
                string hash = string.Empty;
                byte[] bytes = shaManager.ComputeHash(Encoding.ASCII.GetBytes(s), 0, Encoding.ASCII.GetByteCount(s));
                foreach (byte b in bytes)
                {
                    hash += b.ToString("x2");
                }

                return hash;
            }
        }

        /// <summary>
        /// Create new hash string using SHA512
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToSHA512(this string s)
        {
            using (SHA512 shaManager = new SHA512Managed())
            {
                string hash = string.Empty;
                byte[] bytes = shaManager.ComputeHash(Encoding.ASCII.GetBytes(s), 0, Encoding.ASCII.GetByteCount(s));
                foreach (byte b in bytes)
                {
                    hash += b.ToString("x2");
                }

                return hash;
            }
        }

        /// <summary>
        /// Create new hash string using MD5
        /// </summary>
        /// <remarks>
        /// Resource from https://stackoverflow.com/questions/11454004/calculate-a-md5-hash-from-a-string
        /// </remarks>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToMD5(this string s)
        {
            // Use input string to calculate MD5 hash
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(s);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }

                return sb.ToString();
            }
        }

        public static string ToRandomString(this string s, int length){
            const string chars = "abcdefghijklmnopqrstuvxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            
            return new string(System.Linq.Enumerable.Repeat(chars, length)
            .Select(s => s[new Random().Next(s.Length)]).ToArray());
        }

        public static bool NotNullOrEmpty(this string s)
        {
            return !string.IsNullOrWhiteSpace(s);
        }
    }
}