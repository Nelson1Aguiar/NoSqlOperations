using MongoDB.Driver;
using NoSqlOperations.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSqlOperations.Interfaces
{
    public interface IConnectionNoSql
    {
        public dynamic GenerateConnection(ConnectionTypeNoSql connectionType);

    }
}
