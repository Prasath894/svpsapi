using System;
using System.Data;
using System.Data.SqlClient;
using ActivityManagementSystem.Domain.AppSettings;

namespace ActivityManagementSystem.DAL.Infrastructure
{
    public sealed class DataBaseConnection : IDataBaseConnection
    {
        public DataBaseConnection(AppSettings appsettings)
        {
            Connection = new SqlConnection(appsettings.ConnectionInfo.TransactionDatabase);
        }
        public IDbConnection Connection { get; }
    }
}