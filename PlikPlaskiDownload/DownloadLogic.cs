using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PlikPlaskiDownload.Pobieranie;

namespace PlikPlaskiDownload
{
    internal class DownloadLogic
    {
        private DateTime polish_local_time;
        private string format;
        private string save_path;
        public DownloadLogic(string file_format, string file_save_path)
        {
            polish_local_time = DateTime.Now;
            format = file_format;
            save_path = file_save_path;
        }

        public DownloadLogic(string file_format)
        {
            polish_local_time = DateTime.Now;
            format = file_format;
            save_path = Get_Save_Path();
        }

        public FlatFile Invoke_Logic()
        {
            // check for 7z presence
            if (!Check_If_File_Present(this.save_path))
            {
                save_path = Download_Flat_File(Get_Flat_File_Url());
                Console.WriteLine("Finished downloading");
            }
            else
            {
                Console.WriteLine("Todays file {0} present, skipping download", save_path);
            }
            
            FlatFile? flatfile = Load_json(save_path);

            if (flatfile == null)
            {
                Console.WriteLine("Deserialized json is null.");
                Console.WriteLine("Failed to load json.");
                Environment.Exit(1);
                //throw new NoNullAllowedException("Deserialized json is null.");
            }
            else
            {
                Console.WriteLine("Loaded json");
            }


            return flatfile;
        }

        public string Get_Save_Path()
        {
            return string.Format("./{0}{1}", this.polish_local_time.ToString("yyyyMMdd"), this.format);
        }

        private string File_Name_To_Json(string file_name)
        {
            return file_name.Substring(0, save_path.IndexOf(this.format)) + ".json";
        }

        public string Get_Flat_File_Url(string prefix_url = "https://plikplaski.mf.gov.pl/pliki//")
        {
            return prefix_url + this.polish_local_time.ToString("yyyyMMdd") + this.format;
        }

        public bool Check_If_File_Present(string file_path)
        {
            if (File.Exists(file_path)){
                return true;
            }
            else
            {
               return false;
            }
        }

        public string Download_Flat_File(string url)
        {
            var uri = new Uri(url);

            var save_path = string.Format("./{0}{1}", this.polish_local_time.ToString("yyyyMMdd"), this.format);
            using (var client = new HttpClient())
            {
                Console.WriteLine("Downloading {0} file: {1}", this.polish_local_time.ToString("dd/MM/yyyy"), save_path);
                try // handle internet exceptions
                {
                    using (var s = client.GetStreamAsync(uri))
                    {
                        using (var fs = new FileStream(save_path, FileMode.OpenOrCreate))
                        {
                            
                            s.Result.CopyTo(fs);
                            Console.WriteLine("Saved result in " + Path.GetFullPath(save_path));
                            return save_path;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Downloading not successful");
                    Console.WriteLine(e.Message);
                    Environment.Exit(1);
                    return "";
                }
            }
        }

        public FlatFile? Load_json(string save_path)
        {
            string to_json_save_name = "";

            if (!save_path.EndsWith(".json"))
            {
                to_json_save_name = (File_Name_To_Json(save_path));
            }
            else
            {
                to_json_save_name = save_path;
            }

            bool json_present = Check_If_File_Present(File_Name_To_Json(save_path));

            if (save_path.EndsWith(this.format) && !json_present) // from 7z if not present already
            {
                ExtractFile(save_path);
            }
            else if(json_present){
                Console.WriteLine("Json file already present, skipping 7z extraction");
            }

            using (StreamReader r = new StreamReader(to_json_save_name, Encoding.Default, true))
            {
                string json = r.ReadToEnd();
                try
                {
                    FlatFile? flatfile = JsonConvert.DeserializeObject<FlatFile>(json);
                    return flatfile;
                }
                catch (Exception e) {
                    Console.WriteLine("Json deserialization not successful.");
                    Console.WriteLine(e.Message);
                    Environment.Exit(1);
                    return null;
                }
                
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
            catch (System.ComponentModel.Win32Exception e)
            {
                Console.WriteLine("Please install 7zip for console version or manually extract the downloaded file in {0}.", Path.GetFullPath(sourceArchive));
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
            catch (System.Exception e)
            {
                //handle error
                Console.WriteLine("Error while extracting file.");
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
        }
    }
}



