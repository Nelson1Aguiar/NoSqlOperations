using Microsoft.Extensions.Configuration;
using NoSqlOperations.Enum;
using NoSqlOperations.Interfaces;

namespace NoSqlOperations.Connector
{
    public class ConnectionNoSqlProvider
    {
        private static dynamic connectionCurrent;
        private static ConnectionTypeNoSql CurrentTypeInstance;
        private static readonly object _lock = new object();

        public static dynamic GetConnection(ConnectionTypeNoSql connectionType, IConnectionNoSql connection)
        {
            if (CurrentTypeInstance != connectionType)
                connectionCurrent = null;

            if (connectionCurrent == null)
            {
                lock (_lock)
                {
                    connectionCurrent = connection.GenerateConnection(connectionType);
                    CurrentTypeInstance = connectionType;
                }

            }
            return connectionCurrent;
        }
    }
}
