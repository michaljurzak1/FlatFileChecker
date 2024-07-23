using DatabaseConnection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PlikPlaskiCheck
{
    internal class CheckDataSourceFactory : DataSourceFactoryAbstract
    {
        IConnection connection;
        public CheckDataSourceFactory(IConnection connection) : base(connection)
        {
            this.connection = connection;
        }
        public bool Is_Record_In_Table(string tableName, string value, string columnName = "value")
        {
            DataTable dt = connection.ExecuteQuery($"SELECT * FROM {tableName} WHERE {columnName} = '{value}'");

            if (dt.Rows.Count == 1)
                return true;
            else if (dt.Rows.Count == 0)
                return false;
            else
                throw new DataException("More than one record in table");
        }
        /*
        private int GetNumberOfRecords(string tableName)
        {
            DataTable dt = connection.ExecuteQuery($"SELECT COUNT(*) FROM {tableName}");
            if (dt.Rows.Count == 0)
                throw new DataException("No data in DB");

            return int.Parse(dt.Rows[0][0].ToString());
        }

        private List<string> GetTableColumnValues(string tableName, string columnName)
        {
            DataTable dt = connection.ExecuteQuery($"SELECT * FROM {tableName}");
            if (dt.Rows.Count == 0)
                throw new DataException("No data in DB");
            List<string> s = dt.AsEnumerable().Select(x => x["values"].ToString()).ToList();


        }

        public string CheckDateNipMask(string date, string nip, string mask)
        {
            string tableName = "MaskiPodatnikaSHA512";
            string hashString = GetSha512(date, nip, mask);

            // all hashes from date + nip + ALL MASKS
            string[] hashStrings = new string[GetNumberOfRecords("Maski")];

            for (int i = 0; i < hashStrings.Length; i++)
            {
                hashStrings[i] = GetSha512(date, nip, );
            }

            // create all possible hashes from date + nip + ALL MASKS
            CreateMaskTable(tableName, hashStrings);

            return "";
        }

        private void CreateMaskTable(string tableName, string[] hashStrings)
        {
            connection.ExecuteNonQuery($"CREATE TABLE IF NOT EXISTS {tableName} (id INTEGER PRIMARY KEY AUTOINCREMENT, value VARCHAR(128))");
        }
        */
        public string? CheckDateNipNrb(string date, string nip, string nrb)
        {
            nip = nip.Replace(" ", "");
            nrb = nrb.Replace(" ", "");

            if (date.Length != 8 || nip.Length != 10 || nrb.Length != 26)
                throw new ArgumentException("Invalid date, nip or nrb");

            string hashString = GetSha512(date, nip, nrb);
            if (Is_Record_In_Table("SkrotyPodatnikowCzynnych", hashString))
                return "SkrotyPodatnikowCzynnych";

            if (Is_Record_In_Table("SkrotyPodatnikowZwolnionych", hashString))
                return "SkrotyPodatnikowZwolnionych";

            return null;
        }

        #region sha512 logic
        private static string GetSha512(string date, string nip, string nrb)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] hashBytes = Encoding.UTF8.GetBytes(date + nip + nrb);

                string hashString = "";
                int iterations = 5000;

                for (int i = 0; i < iterations; i++)
                {
                    hashBytes = sha512.ComputeHash(hashBytes);
                    hashString = FromHashByteToString(hashBytes);
                    hashBytes = Encoding.UTF8.GetBytes(hashString);
                }

                return hashString;
            }
        }

        private static string FromHashByteToString(byte[] hashBytes)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                builder.Append(hashBytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
        #endregion sha512 logic
    }
}
