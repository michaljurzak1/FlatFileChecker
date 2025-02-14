﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.Data.SqlClient;
using System.Data.Common;
using System.Transactions;

namespace DatabaseConnection
{
    public sealed class SqliteDB : IConnection
    {
        private SqliteConnection connection;
        private string DbName;
        private string DbPath;
        private bool isMock;

        public SqliteDB(bool onlyRead=true, string dbPath="C:/DatabaseSqlite", string dbName="flatfile.db", bool mock=false)
        {
            isMock = mock;
            DbPath = dbPath;
            DbName = dbName;
            SqliteCacheMode cacheMode = SqliteCacheMode.Private;
            Connect(onlyRead, dbPath, mock);
        }

        ~SqliteDB()
        {
            if(!Close())
            {
                throw new DataException("Error closing database connection.");
            }
        }

        public bool Connect(bool onlyRead, string dbPath, bool mock)
        {
            try
            {
                dbPath = Path.Combine(new string[] { dbPath, DbName });

                if (File.Exists(dbPath) && !mock)
                {
                    connection = new SqliteConnection($"Data Source={dbPath};Pooling=False");
                    connection.Open();
                    Console.WriteLine("Connected to database");
                }
                else if (onlyRead)
                {
                    throw new DataException("Database not found");
                }
                else
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(dbPath));
                    File.Create(dbPath).Close();
                    connection = new SqliteConnection($"Data Source={dbPath};Pooling=False");
                    connection.Open();
                    Console.WriteLine("Created and connected database");
                }
            }
            catch (SqliteException e)
            {
                Console.WriteLine("Error connecting to database. Exiting");
                Console.WriteLine(e.Message);
                Environment.Exit(1);
                return false;
            }
            return true;
        }

        public bool Close()
        {
            if (this.connection != null)
            {
                this.connection.Close();
                this.connection.Dispose();
            }
            if (isMock)
            {
                try
                {
                    string dbPath = Path.Combine(new string[] { DbPath, DbName });
                    File.Delete(dbPath);
                    Console.WriteLine("Mock database file deleted");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Mock database file could not be deleted. Close all programs that use this file.");
                    throw e;
                }
            }

            return true;
        }

        public DataTable ExecuteQuery(string query)
        {
            return Read<SqliteConnection>(query);
        }

        private DataTable Read<T>(string query) where T : IDbConnection, new()
        {
            using (var conn = new T())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = query;
                    cmd.Connection = connection;
                    cmd.Connection.Open();
                    var table = new DataTable();
                    table.Load(cmd.ExecuteReader());
                    return table;
                }
            }
        }

        public int ExecuteNonQuery(string query, params IDbDataParameter[]? parameters)
        {
            using (var command = this.connection.CreateCommand())
            {
                command.CommandText = query;
                if (parameters != null && parameters.Length > 0)
                {
                    foreach (var param in parameters)
                    {
                        command.Parameters.Add(param);
                    }
                    return command.ExecuteNonQuery();
                }
                return new SqliteCommand(query, this.connection).ExecuteNonQuery();
            }
        }

        public IDbDataParameter CreateParameter(string name, object value)
        {
            return new SqliteParameter(name, value);
        }

        public void BulkInsert(Dictionary<string, string[]> tableDataPairs)
        {
            foreach (var pair in tableDataPairs)
            {
                using (SqliteTransaction transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = $"INSERT INTO {pair.Key} (value) VALUES ($value)";

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "$value";
                    command.Parameters.Add(parameter);

                    foreach (var value in pair.Value)
                    {
                        parameter.Value = value;
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
            }
        }
    }
}