using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlikPlaskiDownload
{
    internal interface IConnection
    {
        bool Connect();
        bool Close();
        DataTable ExecuteQuery(string query);

        int ExecuteNonQuery(string query, params IDbDataParameter[]? parameters);

        IDbDataParameter CreateParameter(string name, object value);

        void BulkInsert(Pobieranie.FlatFile flatfile); 

    }
}
