using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using NoSqlOperations.Enum;
using NoSqlOperations.Interfaces;

namespace NoSqlOperations.Connection
{
    public class ConnectionMongoDB : IConnectionMongoDB
    {
        private readonly IConfiguration _configuration;

        public ConnectionMongoDB(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public MongoClient? GenerateConnection(ConnectionTypeNoSql connectionType)
        {
            try
            {
                string keyName = connectionType.ToString();
                string connectionString = _configuration.GetConnectionString(keyName);
                MongoClient? connection = new MongoClient(connectionString);
                return connection;
            }
            catch (Exception ex)
            {
                Logger.SaveLog(ex.Message);
                return null;
            }
        }

        public string GetDataBase(ConnectionTypeNoSql connectionType)
        {
            try
            {
                string keyName = connectionType.ToString();
                string dataBase = _configuration.GetConnectionString(keyName);
                return dataBase;
            }
            catch (Exception ex)
            {
                Logger.SaveLog(ex.Message);
                return string.Empty;
            }
        }
    }
}
