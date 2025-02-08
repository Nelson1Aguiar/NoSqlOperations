using NoSqlOperations.Enum;
using StackExchange.Redis;

namespace NoSqlOperations.Interfaces
{
    public interface IConnectionRedis
    {
        public IDatabase? GenerateConnection(ConnectionTypeNoSql connectionType);
    }
}
