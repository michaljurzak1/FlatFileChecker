using Newtonsoft.Json;

namespace FlatFileCheck.Tests
{
    class MinisterstwoFinansowApi
    {
        public static async Task<bool> GetAccount(string nip, string nrb)
        {
            try
            {
                var result = await CheckNip(nip, nrb);
                if (result.result.AccountAssigned == "TAK")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw new HttpRequestException(e.Message);
            }
        }

        static async Task<EntityCheckResponse> CheckNip(string nip, string nrb)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = $"https://wl-api.mf.gov.pl/api/check/nip/{nip}/bank-account/{nrb}";
                HttpResponseMessage response = await client.GetAsync(url);

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<EntityCheckResponse>(responseBody);
            }
        }
    }

    public class EntityCheckResponse
    {
        public Result result { get; set; }
    }

    public class Result
    {
        public string AccountAssigned { get; set; }
        public string RequestId { get; set; }
    }
}
