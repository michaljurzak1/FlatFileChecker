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
using DatabaseConnection;

namespace FlatFileDownload
{
    public static partial class Download
    {
        private static string format = ".7z";

        public static void Main(string[]? args)
        {
            InvokeDownload();
        }

        public static bool InvokeDownload()
        {
            try
            {
                DownloadDataSourceFactory factory = new DownloadDataSourceFactory(new SqliteDB(false));

                #region Download

                if (!factory.CheckFlatFileAvailable(DateTime.Now))
                {
                    Console.WriteLine("No new data available, exiting.");
                    Environment.Exit(0);
                }
                DownloadLogic logic = new DownloadLogic(format);

                FlatFile flatfile = logic.Invoke_Logic();

                #endregion Download

                factory.SaveFlatFile(flatfile);

                logic.DeleteUsedFiles();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}