using MongoDB.Driver;
using NoSqlOperations.Enum;

namespace NoSqlOperations.Interfaces
{
    public interface IConnectionMongoDB
    {
        public MongoClient? GenerateConnection(ConnectionTypeNoSql connectionType);
        public string GetDataBase(ConnectionTypeNoSql connectionType);
    }
}
