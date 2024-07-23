using DatabaseConnection;
using System.Security.Cryptography;
using System.Text;

namespace PlikPlaskiCheck
{
    internal static partial class Checking
    {
        static void Main(string[] args)
        {
            /*
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: PlikPlaskiCheck.exe <nip (10 characters)> <nrb (26 characters)>");
                Environment.Exit(1);
            }
            */
            ArgsChecking(args);

            string date = DateTime.Now.ToString("yyyyMMdd");
            string nip = args[0];
            string nrb = args[1];

            CheckDataSourceFactory factory = new CheckDataSourceFactory(new SqliteDB(true));

            Console.WriteLine("Checking for date:{0}, nip:{1}, nrb:{2}", date, nip, nrb);

            #region Check Date Nip Nrb
                        
            try
            {
                if (!factory.IsDataValid())
                {
                    Console.WriteLine("Latest Data is set to deleted.");
                    Environment.Exit(1);
                }

                // Check for size of the database
                Console.WriteLine($"Database size: {factory.CountData()}");
                
                // Final check for data
                Console.WriteLine(factory.CheckAccount(date, nip, nrb));
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Exiting.");
                Environment.Exit(1);
            }
            
            #endregion Check Date Nip Nrb
        }

        private static void ArgsChecking(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: PlikPlaskiCheck.exe <nip (10 characters)> <nrb (26 characters)>");
                Environment.Exit(1);
            }
            // need another statement if args length is shorter than 2
            if (args[0].Length != 10 || args[1].Length != 26)
            {
                Console.WriteLine("Usage: PlikPlaskiCheck.exe <nip (10 characters)> <nrb (26 characters)>");
                Environment.Exit(1);
            }
        }
    }
}