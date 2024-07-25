using DatabaseConnection;
using Microsoft.Data.Sqlite;
using PlikPlaskiDownload;

namespace PlikPlaskiCheck.Tests
{
    [TestClass]
    public class CheckDataSourceFactoryTests
    {
        string[] skrotyPodatnikowCzynnych = new string[]
            {
                "140bebae8e72c6d3bbe805244870e3cb72f8408d6c6ea0eaa012347aaa686f3c94e63ff4e3fb3dd1297aa0337a03375deef687fbffca36ca5fffb1ed0d142774",
                "0000037ffb0aac44690725f9968c1740c2d5c8780cfbd3ab85d60565fdf64e5da2066f18caf470463c66d18abd52b9c9c9b526501a0b9db319cd02d2b32c1151",
                "0000123602993c214cf508b75276a5d8c18aa96d9e806fb35dce9084768c73aaf423eda9517854161514ee04918c8e8b5aba5b5ded408ec86089f93cf65daa8c",
                "0000139041e8726290e517ea01a8b011c84ed19c89ab764258b4fc35cfc81dc50188f546aece9c00cff881c7ab23a6078aa2ce645fdb6255921d4cd64a6bcabd",
            };

        string[] skrotyPodatnikowZwolnionych = new string[]
        {
            "000034b37dd7ff72a1509f7f3fe1df4ce20105e40ddd663b00249158d993b938a1cb721bf6d60f91f5c43d0c0c3708491878d0f4c2e0060bb09613fc0c5cf981",
            "000055322b1ff5d4202b2bf1fd27ae27edd0e0ab5b66a53252dfe6d260184347e6d7d7b9b681ed3ea6ae3a946ce8a468e7072d85a5c1a74c257baafaf6cfdf73",
            "db387083c45fa318d4c7a13a7d7e1375d0ec0d4fe49479578574fd82b5408b80382bc7e9abbe561e07a05f0915419fbf53e4674491edea63f1573702504b4487",        
            "000094a9b9d5769b1372acc450d700b2cf60363ea1d7a872183c6b6f74d5b876d1a1df52775b6bd25619cde14cb392471f1be5802e4712e1fd63363215b9f743"
        };

        string[] maski = new string[]
        {
                "XX10100055YYYXXXXXXXXXXXXX",
                "XX10100068YYYYXXXXXXXXXXXX",
                "XX10200032YYYXXXXXXXXXXXXX",
                "XX12406960YYYXXXXXXXXXXXXX"
        };

        private static string DbPath = ".";
        private static string DbName = "plikplaskiMock.db";
        private static string MockDbName = Path.Combine(new string[] { DbPath, DbName });
        
        IConnection SQLiteconnection;
        private const string generationDate = "20240725";
        private string nTransformations = "5000";

        [TestInitialize]
        public void Init()
        {
            var tableDataPairs = new Dictionary<string, string[]>
                {
                    { "SkrotyPodatnikowCzynnych", skrotyPodatnikowCzynnych },
                    { "SkrotyPodatnikowZwolnionych", skrotyPodatnikowZwolnionych },
                    { "Maski", maski }
                };

            SQLiteconnection = new SqliteDB(false, DbPath, DbName, true);

            DownloadDataSourceFactory downloadFactory = new DownloadDataSourceFactory(SQLiteconnection);

            SQLiteconnection.BulkInsert(tableDataPairs);
            downloadFactory.DaneInsertLoc(generationDate, nTransformations);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // comment this line to keep the database after the test
            // however, it is not guaranteed that all test will have access
            // and will throw error
            SQLiteconnection.Close();
        }

        [TestMethod]
        public void IsDataValidTest()
        {
            var factory = new CheckDataSourceFactory(SQLiteconnection);

            var result = factory.IsDataValid(generationDate);

            Assert.IsTrue(result, "IsDataValid not working properly.");
        }

        [TestMethod]
        public void CountDataTest()
        {
            var factory = new CheckDataSourceFactory(SQLiteconnection);

            var result = factory.CountData();

            Assert.AreEqual(skrotyPodatnikowCzynnych.Length + skrotyPodatnikowZwolnionych.Length, 
                result, "CountData not working properly.");
        }

        [TestMethod]
        [DataRow("5512393471", "51 1950 0001 2006 0073 4183 0002", "\nReal Account in SkrotyPodatnikowCzynnych")]
        [DataRow("7740001454", "71124069606666000007118600", "\nVirtual Account in SkrotyPodatnikowZwolnionych")]
        public void CheckAccountTest(string nip, string nrb, string response)
        {
            var factory = new CheckDataSourceFactory(SQLiteconnection);

            var result = factory.CheckAccount(generationDate, nip, nrb);
            Console.WriteLine(result.Equals(response));

            Assert.AreEqual(response, result, "AccountTest not working properly.");
        }

        [TestMethod]
        [DataRow("SkrotyPodatnikowCzynnych", "0000123602993c214cf508b75276a5d8c18aa96d9e806fb35dce9084768c73aaf423eda9517854161514ee04918c8e8b5aba5b5ded408ec86089f93cf65daa8c", "value")]
        [DataRow("SkrotyPodatnikowZwolnionych", "000034b37dd7ff72a1509f7f3fe1df4ce20105e40ddd663b00249158d993b938a1cb721bf6d60f91f5c43d0c0c3708491878d0f4c2e0060bb09613fc0c5cf981", "value")]
        [DataRow("Maski", "XX12406960YYYXXXXXXXXXXXXX", "value")]
        [DataRow("Dane", generationDate, "generatingDate")]
        public void CheckRecordInTable(string tableName, string value, string columnName)
        {
            var factory = new CheckDataSourceFactory(SQLiteconnection);

            var result = factory.Is_Record_In_Table(tableName, value, columnName);

            Assert.IsTrue(result, "CheckRecordInTable not working properly.");
        }

        /*
        [TestMethod]
        [DataRow("5252344078", "37103015080000000504162044")]
        public async Task CheckApi(string nip, string nrb)
        {
            bool? result = await MinisterstwoFinansowApi.GetAccount(nip, nrb);
            if (result == null)
            {
                Console.WriteLine("Error regarding Api connection.");
                Assert.Fail();
            }
            Assert.IsTrue(result.Value, "Api check failed.");
            Console.WriteLine(result);
        }*/
    }
}
