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

namespace PlikPlaskiDownload
{
    internal static partial class Pobieranie
    {
        private static string format = ".7z";
        private static string save_path = "./20240719.7z";
        
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

            //Console.WriteLine(flatfile.naglowek.schemat); // testowe, sprawdzenie czy parsuje json

        }
    }
}