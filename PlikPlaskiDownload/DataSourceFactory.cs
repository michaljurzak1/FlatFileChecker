using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
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
            Initiate_DB(); //initiates all tables or truncates SPCzynnych, SPZwolnionych, Maski tables
        }

        public bool CheckFlatFileAvailable(DateTime now)
        {
            // Ensure one minute delay after 00:00
            try
            {
                var last_date = now > Get_Last_date().AddMinutes(1);
                return last_date;
            } catch (DataException e)
            {
                return true;
            }
        }

        public void SaveFlatFile(Pobieranie.FlatFile flatfile)
        {
            // before anything need to truncate and update
            TruncateTables();
            UpdateDane();

            Console.WriteLine("Saving new data");

            connection.BulkInsert(flatfile);
            DaneInsert(flatfile.naglowek.dataGenerowaniaDanych, flatfile.naglowek.liczbaTransformacji);

            Console.WriteLine("Saved");
        }

        private void DaneInsert(string generatingDate, string nTransformations)
        {
            // Insert into Dane table nessesary data
            IDbDataParameter[] parameters = new IDbDataParameter[4];
            parameters[0] = connection.CreateParameter("$insertingDate", DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss")); // 2024.07.19 19:00:00
            parameters[1] = connection.CreateParameter("$generatingDate", generatingDate);
            parameters[2] = connection.CreateParameter("$deleted", 0);
            parameters[3] = connection.CreateParameter("$nTransformations", int.Parse(nTransformations));
            connection.ExecuteNonQuery(
                @"INSERT INTO Dane 
                (insertingDate, generatingDate, deleted, nTransformations) 
                VALUES ($insertingDate,$generatingDate,$deleted,$nTransformations)", 
                parameters
                );
        }

        private void UpdateDane()
        {
            // Update all to deleted
            connection.ExecuteNonQuery("UPDATE Dane SET deleted = 1");
        }

        private void TruncateTables()
        {
            connection.ExecuteNonQuery("DELETE FROM SkrotyPodatnikowCzynnych");
            connection.ExecuteNonQuery("DELETE FROM SkrotyPodatnikowZwolnionych");
            connection.ExecuteNonQuery("DELETE FROM Maski");

            // Reset autoincrement index for each table
            connection.ExecuteNonQuery("DELETE FROM sqlite_sequence WHERE name = 'SkrotyPodatnikowCzynnych';");
            connection.ExecuteNonQuery("DELETE FROM sqlite_sequence WHERE name = 'SkrotyPodatnikowZwolnionych';");
            connection.ExecuteNonQuery("DELETE FROM sqlite_sequence WHERE name = 'Maski';");
        }

        private bool Initiate_DB()
        {
            // create tables if not exist
            connection.ExecuteNonQuery(
                @"CREATE TABLE IF NOT EXISTS Dane 
                (id INTEGER PRIMARY KEY AUTOINCREMENT, 
                insertingDate VARCHAR(19) NOT NULL, 
                generatingDate VARCHAR(8) NOT NULL, 
                deleted INT(1) NOT NULL, 
                nTransformations INT NOT NULL)"
            );
            
            connection.ExecuteNonQuery(
                @"CREATE TABLE IF NOT EXISTS SkrotyPodatnikowCzynnych 
                (id INTEGER PRIMARY KEY AUTOINCREMENT, 
                value VARCHAR(128) NOT NULL)"
            );

            connection.ExecuteNonQuery(
                @"CREATE TABLE IF NOT EXISTS SkrotyPodatnikowZwolnionych 
                (id INTEGER PRIMARY KEY AUTOINCREMENT, 
                value VARCHAR(128) NOT NULL)"
            );

            connection.ExecuteNonQuery(
                @"CREATE TABLE IF NOT EXISTS Maski 
                (id INTEGER PRIMARY KEY AUTOINCREMENT, value VARCHAR(26) NOT NULL)"
            );

            return true;
        }

        private DateTime Get_Last_date()
        {
            // check last date from column: dataGenerowaniaDanych (20240719)
            DataTable dt = connection.ExecuteQuery("SELECT generatingDate FROM Dane WHERE deleted = 0 ORDER BY id DESC LIMIT 1");
            if (dt.Rows.Count == 0)
            {
                throw new DataException("No data in DB");
            }
            string? date = dt.Rows[0]["generatingDate"].ToString();
            if (date == null)
            {
                throw new DataException("No data in DB");
            }
            return DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);
            
            // for testing:
            //DateTime date = new DateTime(2024, 7, 18); // 19
            //return date;
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
    }
}
