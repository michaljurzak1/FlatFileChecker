using DatabaseConnection;
using System.Security.Cryptography;
using System.Text;

namespace PlikPlaskiCheck
{
    internal static partial class Checking
    {
        private static string format = ".7z";

        static void Main(string[] args)
        {
            // program for searching DB
            CheckDataSourceFactory factory = new CheckDataSourceFactory(new SqliteDB(true));

            //check database if there is data
            #region Check Sha512 Date Nip Nrb

            string date = DateTime.Now.ToString("yyyyMMdd");
            string nip = "6750001923";
            string nrb = "77124022941111001073675085";
            
            Console.WriteLine("Checking for date:{0}, nip:{1}, nrb:{2}", date, nip, nrb);

            try
            {
                string? sha512 = factory.CheckDateNipNrb(date, nip, nrb);
                if (sha512 != null)
                {
                    Console.WriteLine(sha512);
                }
                else
                {
                    Console.WriteLine("No record found");
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Exiting.");
                Environment.Exit(1);
            }
            
            

            //Console.WriteLine(flatfile.naglowek.schemat); // testowe, sprawdzenie czy parsuje json
            #endregion Check Sha512 Date Nip Nrb


        }

    }
}