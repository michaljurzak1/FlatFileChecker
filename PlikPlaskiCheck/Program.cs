using DatabaseConnection;
using System.Security.Cryptography;
using System.Text;

namespace PlikPlaskiCheck
{
    internal static partial class Checking
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: PlikPlaskiCheck.exe <nip (10 characters)> <nrb (26 characters)>");
                Environment.Exit(1);
            }

            string date = DateTime.Now.ToString("yyyyMMdd");
            string nip = args[0];
            string nrb = args[1];

            /*
             Some cases:
            1:
            string nip = "6750001923";
            string nrb = "23124069608070101200041035";
             */

            CheckDataSourceFactory factory = new CheckDataSourceFactory(new SqliteDB(true));

            #region Check Sha512
            /*
            if (CheckDataSourceFactory.GetSha512("20191018", "1134679109", "XX72123370XXXXXXX022XXXXXX")
                == "d3dfed802034d198b484c9f19e43c1b7540c3a7808503d01a5ccedbb169012bee6a77979ed46b27f5de2bee0d22eb7c7ca9522dfa92e465999e68e9906e01425")
                Console.WriteLine("Sha512 is correct");
            */
            #endregion Check Sha512

            Console.WriteLine("Checking for date:{0}, nip:{1}, nrb:{2}", date, nip, nrb);

            #region Check Date Nip Nrb
                        
            try
            {
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
    }
}