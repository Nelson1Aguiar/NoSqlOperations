using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using NoSqlOperations.Enum;
using NoSqlOperations.Interfaces;

public class ConnectionMongoDB : IConnectionMongoDB
{
    private readonly IConfiguration _configuration;
    private static readonly object _lock = new();
    private static MongoClient? _mongoClient;

    public ConnectionMongoDB(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public MongoClient? GenerateConnection(ConnectionTypeNoSql connectionType)
    {
        try
        {
            if (_mongoClient == null)
            {
                lock (_lock)
                {
                    if (_mongoClient == null)
                    {
                        string keyName = connectionType.ToString();
                        string connectionString = _configuration.GetConnectionString(keyName);
                        _mongoClient = new MongoClient(connectionString);
                    }
                }
            }

            return _mongoClient;
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
            return _configuration.GetConnectionString(keyName);
        }
        catch (Exception ex)
        {
            Logger.SaveLog(ex.Message);
            return string.Empty;
        }
    }
}
