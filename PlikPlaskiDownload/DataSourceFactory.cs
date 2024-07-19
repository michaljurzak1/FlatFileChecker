using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlikPlaskiDownload
{
    internal class DataSourceFactory
    {
        IConnection connection;

        public DataSourceFactory(IConnection connection)
        {
            this.connection = connection;
            Initiate_DB();
        }

        public bool CheckFlatFileAvailable(DateTime now)
        {
            return now > Get_Last_date();
        }

        public void SaveFlatFile(Pobieranie.FlatFile flatfile)
        {
            Console.WriteLine("Inserting flatfile into database");
            foreach (var item in flatfile.skrotyPodatnikowCzynnych)
            {
                IDbDataParameter[] param = new IDbDataParameter[2];
                param[0] = connection.CreateParameter("@shortcut", item);
                param[1] = connection.CreateParameter("@generatingDate", flatfile.naglowek.dataGenerowaniaDanych);
                connection.ExecuteNonQuery("INSERT INTO SkrotyPodatnikowCzynnych (shortcut, generatingDate) VALUES (@shortcut, @generatingDate)", param);
                break;
            }
            Console.WriteLine("Saved  SkrotyPodatnikowCzynnych");
            //connection.ExecuteNonQuery("INSERT INTO SkrotyPodatnikowCzynnych (shortcut, generatingDate) VALUES (?,?)", );

            //throw new NotImplementedException();
        }

        private bool Initiate_DB()
        {
            //connection.ExecuteQuery("CREATE DATABASE plikplaski;");
            connection.ExecuteNonQuery("CREATE TABLE IF NOT EXISTS SkrotyPodatnikowCzynnych (id INTEGER PRIMARY KEY AUTOINCREMENT, shortcut VARCHAR(128) NOT NULL, generatingDate VARCHAR(8) NOT NULL)");
            connection.ExecuteNonQuery("CREATE TABLE IF NOT EXISTS SkrotyPodatnikowZwolnionych (id INTEGER PRIMARY KEY AUTOINCREMENT, shortcut VARCHAR(128) NOT NULL, generatingDate VARCHAR(8) NOT NULL)");
            connection.ExecuteNonQuery("CREATE TABLE IF NOT EXISTS Maski (id INTEGER PRIMARY KEY AUTOINCREMENT, shortcut VARCHAR(26) NOT NULL, generatingDate VARCHAR(8) NOT NULL)");
            return true;
        }

        private DateTime Get_Last_date()
        {
            // check last date from column: dataGenerowaniaDanych (20240719)
            // for testing:
            DateTime date = new DateTime(2024, 7, 18); // 19
            return date;
        }

        private bool Is_Record_In_DB()
        {
            throw new NotImplementedException();
        }
    }
}
