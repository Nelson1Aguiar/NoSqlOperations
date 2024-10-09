using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using NoSqlOperations.Enum;
using NoSqlOperations.Interfaces;
using StackExchange.Redis;


namespace NoSqlOperations.Connector
{
    public class ConnectionNoSql : IConnectionNoSql
    {
        private readonly IConfiguration _configuration;

        public ConnectionNoSql(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public dynamic GenerateConnection(ConnectionTypeNoSql connectionType)
        {
            try
            {
                dynamic conexao = null;
                string keyName = connectionType.ToString();
                string varConexao = GetConnectionString(keyName);

                switch ((int)connectionType)
                {
                    case 0:
                        conexao = new MongoClient(varConexao);
                        break;
                    case 1:
                        conexao = varConexao;
                        break;
                    case 2:
                        conexao = ConnectionRedis(varConexao);
                        break;
                }
                return conexao;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro de conexão: " + ex.Message);
                return null;
            }
        }

        private string GetConnectionString(string key)
        {
            string connectionString = _configuration.GetConnectionString(key);
            return connectionString;
        }

        private IDatabase ConnectionRedis(string varConexao)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(varConexao);
            IDatabase db = redis.GetDatabase();

            return db;
        }
    }
}
