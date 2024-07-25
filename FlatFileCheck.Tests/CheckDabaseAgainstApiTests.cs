using DatabaseConnection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlatFileCheck.Tests
{
    [TestClass]
    public class CheckDabaseAgainstApiTests
    {
        /*
        [TestMethod]
        [DataRow("5260250274", "10101010100038252231000000")] // Ministerstwo Finansow
        public async Task CheckApi(string nip, string nrb)
        {
            try
            {
                bool result = await MinisterstwoFinansowApi.GetAccount(nip, nrb);
                Assert.IsTrue(result, "Api check failed.");
            }
            catch (Exception e)
            {
                HandleResponseException(e);
            }
        }*/

        private void HandleResponseException(Exception e)
        {
            var match = Regex.Match(e.Message, @"\b\d{3}\b");
            if (match.Success)
            {
                int statusCode = int.Parse(match.Value);
                HandleStatusCode(statusCode);
            }
            else
            {
                Assert.Fail("Api check failed or connection error.");
            }
        }

        [TestMethod]
        [DataRow("5260250274", "10101010100038252231000000")] // Ministerstwo Finansow
        public async Task CheckDbDefaultLocationAgainstApi(string nip, string nrb)
        {
            if (File.Exists("/DatabaseSqlite/flatfile.db"))
            {
                try
                {
                    bool result = await MinisterstwoFinansowApi.GetAccount(nip, nrb);
                    var factory = new CheckDataSourceFactory(new SqliteDB(true));
                    
                    string today = DateTime.Now.ToString("yyyyMMdd");
                    if (!factory.IsDataValid(today))
                    {
                        Assert.Fail("Data not valid in database.");
                        return;
                    }
                    string isInResult = factory.CheckAccount(today, nip, nrb);

                    Assert.AreEqual(isInResult, "\nReal Account in SkrotyPodatnikowCzynnych", "Local database differs with api.");
                    return;
                } 
                catch(HttpRequestException e)
                {
                    HandleResponseException(e);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Assert.Fail("Api check failed or connection error.");
                }
            }
            else
            {
                Assert.Inconclusive("Database not found.");
            }
        }
        private void HandleStatusCode(int statusCode)
        {
            if (statusCode >= 400 && statusCode < 500)
            {
                if (statusCode == 400)
                    Assert.Fail("Api check failed");
                else if (statusCode == 429)
                    Assert.Inconclusive("Api too many requests / limit reached.");

                Assert.Fail("Api user error occurred.");
            }
            else if (statusCode >= 500)
                Assert.Fail("Api server error occurred.");
            else
                Assert.Fail("Api unexpected error occurred.");
        }
    }
}
