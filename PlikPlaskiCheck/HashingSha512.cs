using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PlikPlaskiCheck
{
    public static class HashingSha512
    {
        #region sha512 logic

        public static string GetSha512(string date, string nip, string nrb, int iterations = 5000)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] hashBytes = Encoding.UTF8.GetBytes(date + nip + nrb);

                string hashString = "";

                for (int i = 0; i < iterations; i++)
                {
                    hashBytes = sha512.ComputeHash(hashBytes);
                    hashString = hashBytes.FromHashByteToString();
                    hashBytes = Encoding.UTF8.GetBytes(hashString);
                }

                return hashString;
            }
        }

        // extension method
        private static string FromHashByteToString(this byte[] hashBytes)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                builder.Append(hashBytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
        #endregion sha512 logic
    }
}
