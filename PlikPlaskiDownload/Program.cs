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

namespace PlikPlaskiDownload
{
    internal static partial class Pobieranie
    {
        private static string format = ".7z";
        
        static void Main(string[] args)
        {
            DataSourceFactory factory = new DataSourceFactory(new SqliteDB());
            
            
            if (!factory.CheckFlatFileAvailable(DateTime.Now))
            {
                Console.WriteLine("No new data available, exiting.");
                Environment.Exit(0);
            }
            DownloadLogic logic = new DownloadLogic(format);

            FlatFile flatfile = logic.Invoke_Logic();
            factory.SaveFlatFile(flatfile);

            logic.DeleteUsedFiles();
            
            /*
            Console.WriteLine("Checking for Tema Komputer:");
            string sha512 = CheckSha512(DateTime.Now.ToString("yyyyMMdd"), "5512374568", "92105011001000009030904370");
            //"4356579386"
            //"5512374568"
            //"49584845845845839967467456"
            //"92105011001000009030904370"
            Console.WriteLine(sha512);

            Console.WriteLine(factory.Is_Record_In_Table("SkrotyPodatnikowCzynnych", sha512));
            Console.WriteLine(factory.Is_Record_In_Table("SkrotyPodatnikowZwolnionych", sha512));

            //Console.WriteLine(flatfile.naglowek.schemat); // testowe, sprawdzenie czy parsuje json
            */
        }
        /*
        public static string CheckSha512(string date, string nip, string nrb)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] hashBytes = Encoding.UTF8.GetBytes(date + nip + nrb);
                
                for (int i = 0; i < 5000; i++)
                {
                    hashBytes = sha512.ComputeHash(hashBytes);
                }
                //byte[] hashBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(date + nip + nrb));
                
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        */
    }
}