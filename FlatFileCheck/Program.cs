using DatabaseConnection;
using FlatFileDownload;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace FlatFileCheck
{
    internal static partial class Checking
    {
        static void Main(string[] args)
        {
            ArgsChecking(args);

            string date = DateTime.Now.ToString("yyyyMMdd");
            string nip = args[0];
            string nrb = args[1];

            CheckDataSourceFactory factory;

            try
            {
                factory = new CheckDataSourceFactory(new SqliteDB(true));
            }
            catch
            {
                Console.WriteLine("Database not present. Do you want to download latest file? [Y/N]");
                DownloadOnCmdResponse();

                factory = new CheckDataSourceFactory(new SqliteDB(true));
            }

            Console.WriteLine("Checking for date:{0}, nip:{1}, nrb:{2}", date, nip, nrb);

            #region Check Date Nip Nrb

            try
            {
                if (!factory.IsDataValid(date))
                {
                    Console.WriteLine("Latest Data in Database is not valid. Do you want to download latest file? [Y/N]");
                    
                    if (!DownloadOnCmdResponse())
                    {
                        Console.WriteLine("Do you want to proceed with current data? [Y/N]");

                        string? response = Console.ReadLine();
                        while (response != "Y" && response != "y" && response != "N" && response != "n")
                        {
                            Console.WriteLine("Please enter Y or N.");
                            response = Console.ReadLine();
                        }

                        if (!(response == "Y" || response == "y"))
                        {
                            Console.WriteLine("Exiting.");
                            Environment.Exit(1);
                        }
                    }
                    //Environment.Exit(1);
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
                Console.WriteLine("Usage: FlatFileCheck.exe <nip (10 characters)> <nrb (26 characters)>");
                Environment.Exit(1);
            }
            // need another statement if args length is shorter than 2
            if (args[0].Length != 10 || args[1].Length != 26)
            {
                Console.WriteLine("Usage: FlatFileCheck.exe <nip (10 characters)> <nrb (26 characters)>");
                Environment.Exit(1);
            }
        }

        private static bool DownloadOnCmdResponse() //returns if download was successful
        {
            string? response = Console.ReadLine();
            while (response != "Y" && response != "y" && response != "N" && response != "n")
            {
                Console.WriteLine("Please enter Y or N.");
                response = Console.ReadLine();
            }

            if (response == "Y" || response == "y")
            {
                Console.WriteLine("Downloading latest file.");
                return Download.InvokeDownload();
            }
            else
            {
                Console.WriteLine("Exiting.");
                Environment.Exit(1);
            }
            return false;
        }
    }
}