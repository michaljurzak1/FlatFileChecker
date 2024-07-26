using System.Text.Json;
using Newtonsoft.Json;

namespace FlatFileDownload
{
    public static partial class Download
    {
        public class FlatFile
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
    }
}