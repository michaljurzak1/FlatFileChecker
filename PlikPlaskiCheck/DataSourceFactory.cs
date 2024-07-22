using DatabaseConnection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlikPlaskiCheck
{
    internal class DataSourceFactory : DataSourceFactoryAbstract
    {
        IConnection connection;
        public DataSourceFactory(IConnection connection) : base(connection)
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
    }
}
