using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnection
{
    public abstract class DataSourceFactoryAbstract
    {
        IConnection connection;

        public DataSourceFactoryAbstract(IConnection connection)
        {
            this.connection = connection;
        }

        #region helper methods

        public bool CheckFlatFileAvailable(DateTime now)
        {
            // Ensure one minute delay after 00:00
            try
            {
                var last_date = now.Date > Get_Last_date().AddMinutes(1);
                return last_date;
            }
            catch (DataException e)
            {
                return true;
            }
        }

        protected DateTime Get_Last_date()
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
        }

        #endregion helper methods

        protected void DaneInsert(string generatingDate, string nTransformations)
        {
            // Insert into table Dane necessary data
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
    }
}
