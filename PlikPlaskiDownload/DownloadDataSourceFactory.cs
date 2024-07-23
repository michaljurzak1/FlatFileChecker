using DatabaseConnection;
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
    internal class DownloadDataSourceFactory : DataSourceFactoryAbstract
    {
        IConnection connection;

        public DownloadDataSourceFactory(IConnection connection) : base(connection)
        {
            this.connection = connection;
            Initiate_DB(); //initiates all tables or truncates SPCzynnych, SPZwolnionych, Maski tables
        }

        public void SaveFlatFile(Pobieranie.FlatFile flatfile)
        {
            // before anything need to truncate and update
            TruncateTables();
            UpdateDane();

            Console.WriteLine("Saving new data");

            var tableDataPairs = new Dictionary<string, string[]>
            {
                { "SkrotyPodatnikowCzynnych", flatfile.skrotyPodatnikowCzynnych },
                { "SkrotyPodatnikowZwolnionych", flatfile.skrotyPodatnikowZwolnionych },
                { "Maski", flatfile.maski }
            };

            connection.BulkInsert(tableDataPairs);
            DaneInsert(flatfile.naglowek.dataGenerowaniaDanych, flatfile.naglowek.liczbaTransformacji);

            Console.WriteLine("Saved");
        }

        #region database insertion handling methods

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

        protected bool Initiate_DB()
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
        #endregion database insertion handling methods
    }
}
