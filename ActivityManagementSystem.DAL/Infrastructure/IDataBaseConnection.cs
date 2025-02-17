using System;
using System.Data;

namespace ActivityManagementSystem.DAL.Infrastructure
{
    public interface IDataBaseConnection
    {
        IDbConnection Connection { get; }
    }
}

