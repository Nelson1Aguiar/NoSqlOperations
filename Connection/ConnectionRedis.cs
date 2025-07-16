using Microsoft.Extensions.Configuration;
using NoSqlOperations.Enum;
using NoSqlOperations.Interfaces;
using StackExchange.Redis;

public class ConnectionRedis : IConnectionRedis
{
    private readonly IConfiguration _configuration;
    private static readonly object _lock = new();
    private static ConnectionMultiplexer? _connectionMultiplexer;

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

            if (_connectionMultiplexer == null || !_connectionMultiplexer.IsConnected)
            {
                lock (_lock)
                {
                    if (_connectionMultiplexer == null || !_connectionMultiplexer.IsConnected)
                    {
                        _connectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
                    }
                }
            }

            return _connectionMultiplexer.GetDatabase();
        }
        catch (Exception ex)
        {
            Logger.SaveLog(ex.Message);
            return null;
        }
    }
}
