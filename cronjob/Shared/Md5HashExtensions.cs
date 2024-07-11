using System.Security.Cryptography;
using System.Text;

namespace System
{
    public static class Md5HashExtensions
    {
        // Hash an input string and return the hash as
        // a 32 character hexadecimal string.
        public static string GetMd5Hash(this byte[] input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(input);

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        public static bool VerifyMd5Hash(this byte[] input, string hash)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string GetMd5Hash(this string input)
        {
            return GetMd5Hash(Encoding.Default.GetBytes(input));
        }

        public static bool VerifyMd5Hash(this string input, string hash)
        {
            return VerifyMd5Hash(Encoding.Default.GetBytes(input), hash);
        }
    }
}