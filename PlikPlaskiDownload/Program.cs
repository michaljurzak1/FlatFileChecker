using System;
using System.Formats.Asn1;
using System.IO.Compression;
using System.Net;
using System.Text.Json;
using static System.Net.WebRequestMethods;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;
using System.Reflection.PortableExecutable;
using System.Text;

namespace PlikPlaskiDownload
{
    internal static class Pobieranie
    {
        private static DateTime polish_local_time;
        private static string format = ".7z";
        private static string save_path = "20240716.7z"; // temporary - to delete
        //private static string save_path = "trimmed_json.json"; // temporary - to delete


        static void Main(string[] args)
        {
            polish_local_time = DateTime.Now;
            //save_path = Download_Flat_File(Get_Flat_File_Url());
            //Console.WriteLine(save_path.Substring(0, save_path.IndexOf(".")));
            var flatfile = Load_json(save_path);

            if (flatfile == null)
            {
                Console.WriteLine("Failed to load json");
                throw new NoNullAllowedException("Deserialized json is null.");
            }
            else
            {
                Console.WriteLine("Loaded json");
            }

            Console.WriteLine(flatfile.naglowek.schemat);

        }

        private static string Get_Flat_File_Url(string prefix_url = "https://plikplaski.mf.gov.pl/pliki//")
        {
            return prefix_url + polish_local_time.ToString("yyyyMMdd") + format;
        }
        
        private static string Download_Flat_File(string url)
        {
            var uri = new Uri(url);

            var save_path = string.Format("./{0}{1}", polish_local_time.ToString("yyyyMMdd"), format);
            Console.WriteLine(uri);
            using (var client = new HttpClient())
            {
                using (var s = client.GetStreamAsync(uri))
                {
                    using (var fs = new FileStream(save_path, FileMode.OpenOrCreate))
                    {
                        s.Result.CopyTo(fs);
                        Console.WriteLine("Saved result in " + save_path);
                        return save_path;
                    }
                }
            }
        }

        public static FlatFile Load_json(string save_path)
        {
            string save_name = save_path.Substring(0, save_path.IndexOf(".")) + ".json";
            Console.WriteLine(save_name);
            
            if (save_path.EndsWith(".7z")){
                ExtractFile(save_path);
            }
            
            using (StreamReader r = new StreamReader(save_name, Encoding.Default, true))
            {
                //Console.WriteLine(save_path);
                r.Peek(); // you need this!
                var encoding = r.CurrentEncoding;
                Console.WriteLine("Current encoding: ", encoding);
                string json = r.ReadToEnd();
                FlatFile? flatfile = JsonConvert.DeserializeObject<FlatFile>(json);
                return flatfile;
            }
            
        }

        internal class FlatFile
        {
            [JsonProperty("naglowek")]
            public Naglowek naglowek;

            [JsonProperty("skrotyPodatnikowCzynnych")]
            public string[] skrotyPodatnikowCzynnych;

            [JsonProperty("skrotyPodatnikowZwolnionych")]
            public string[] skrotyPodatnikowZwolnionych;

            [JsonProperty("maski")]
            public string[] maski;

            public class Naglowek
            {
                [JsonProperty("dataGenerowaniaDanych")]
                public string dataGenerowaniaDanych;

                [JsonProperty("liczbaTransformacji")]
                public string liczbaTransformacji;

                [JsonProperty("schemat")]
                public string schemat;
            }
        }

        public static void ExtractFile(string sourceArchive, string destination = ".")
        {
            string zPath = "7za.exe"; //add to proj and set CopyToOuputDir
            try
            {
                ProcessStartInfo pro = new ProcessStartInfo();
                pro.WindowStyle = ProcessWindowStyle.Hidden;
                pro.FileName = zPath;
                pro.Arguments = string.Format("x \"{0}\" -y -o\"{1}\"", sourceArchive, destination);
                Process x = Process.Start(pro);
                x.WaitForExit();
            }
            catch (System.Exception Ex)
            {
                //handle error
                Console.WriteLine(Ex.Message);
            }
        }

    }
}