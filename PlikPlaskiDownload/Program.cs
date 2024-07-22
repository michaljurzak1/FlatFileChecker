using System;
using System.Formats.Asn1;
using System.IO.Compression;
using System.Net;
using static System.Net.WebRequestMethods;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Security.Cryptography;
using System.Security.Principal;

namespace PlikPlaskiDownload
{
    internal static partial class Pobieranie
    {
        private static string format = ".7z";
        
        static void Main(string[] args)
        {
            DataSourceFactory factory = new DataSourceFactory(new SqliteDB());

            #region Download

            if (!factory.CheckFlatFileAvailable(DateTime.Now))
            {
                Console.WriteLine("No new data available, exiting.");
                Environment.Exit(0);
            }
            DownloadLogic logic = new DownloadLogic(format);

            FlatFile flatfile = logic.Invoke_Logic();
            factory.SaveFlatFile(flatfile);

            logic.DeleteUsedFiles();

            #endregion Download

            // testing, delete later
            #region CheckSha512

            string date = DateTime.Now.ToString("yyyyMMdd");
            string nip = "6750001923";
            string nrb = "77124022941111001073675085";
            Console.WriteLine(string.Format("{0}{1}{2}", date, nip, nrb));

            Console.WriteLine("Checking for agh:");
            string sha512 = CheckSha512(date, nip, nrb);

            Console.WriteLine(sha512);

            Console.WriteLine(factory.Is_Record_In_Table("SkrotyPodatnikowCzynnych", sha512));
            Console.WriteLine(factory.Is_Record_In_Table("SkrotyPodatnikowZwolnionych", sha512));

            //Console.WriteLine(flatfile.naglowek.schemat); // testowe, sprawdzenie czy parsuje json
            #endregion CheckSha512
        }

        #region sha512 logic
        public static string CheckSha512(string date, string nip, string nrb)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] hashBytes = Encoding.UTF8.GetBytes(date + nip + nrb);

                string hashString = "";
                int iterations = 5000;

                for (int i = 0; i < iterations; i++)
                {
                    hashBytes = sha512.ComputeHash(hashBytes);
                    hashString = FromHashByteToString(hashBytes);
                    hashBytes = Encoding.UTF8.GetBytes(hashString);
                }

                return hashString;
            }
        }

        private static string FromHashByteToString(byte[] hashBytes)
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