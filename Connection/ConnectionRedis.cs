using Microsoft.Extensions.Configuration;
using NoSqlOperations.Enum;
using NoSqlOperations.Interfaces;
using StackExchange.Redis;

namespace NoSqlOperations.Connector
{
    public class ConnectionRedis : IConnectionRedis
    {
        private readonly IConfiguration _configuration;

        public ConnectionRedis(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDatabase? GenerateConnection(ConnectionTypeNoSql connectionType)
        {
            try
            {
                string keyName = connectionType.ToString();
                string connectionString = _configuration.GetConnectionString(keyName);

                IDatabase? connection = ConnectionRedisProvider(connectionString);
                return connection;
            }
            catch (Exception ex)
            {
                Logger.SaveLog(ex.Message);
                return null;
            }
        }

        private IDatabase? ConnectionRedisProvider(string connectionString)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(connectionString);
            IDatabase? db = redis.GetDatabase();

            return db;
        }
    }
}
