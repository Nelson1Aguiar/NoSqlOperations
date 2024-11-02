using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NoSqlOperations.Connector;
using NoSqlOperations.Enum;
using NoSqlOperations.Interfaces;
using StackExchange.Redis;

namespace NoSqlOperations.Operations
{
    public class RedisOperations : IRedisOperation
    {
        private IDatabase _dataBase;
        private readonly IConnectionNoSql _connectionNoSql;

        public RedisOperations(IConnectionNoSql connectionNoSql)
        {
            _connectionNoSql = connectionNoSql;
            _dataBase = _connectionNoSql.GenerateConnection(ConnectionTypeNoSql.RedisConnection);
        }

        public void SetData<T>(T entity, string redisKey)
        {
            if (_dataBase != null)
            {
                try
                {
                    string jsonEntity = JsonConvert.SerializeObject(entity);
                    _dataBase.StringSet(redisKey, jsonEntity);
                }
                catch (Exception ex)
                {
                    Logger.SaveLog(ex.Message);
                }
            }
        }

        public T? GetData<T>(string redisKey) where T : class
        {
            if (_dataBase != null)
            {
                try
                {
                    string JsonEntity = _dataBase.StringGet(redisKey);
                    T entity = JsonConvert.DeserializeObject<T>(JsonEntity);
                    return entity;
                }
                catch (Exception ex)
                {
                    Logger.SaveLog(ex.Message);
                    return null;
                }
            }
            return null;
        }

        public List<T>? GetAllDataByKey<T>(string redisKey)
        {
            List<T> entities = new List<T>();

            if (_dataBase != null)
            {
                try
                {
                    string pattern = $"*{redisKey}*";
                    string serverName = _dataBase.Multiplexer.Configuration;
                    string formatServerName = FormatServerName(serverName);
                    var server = _dataBase.Multiplexer.GetServer(formatServerName);
                    var keys = server.Keys(pattern: pattern);
                    foreach (var key in keys)
                    {
                        string jsonEntity = _dataBase.StringGet(key);
                        if (jsonEntity != null)
                        {
                            entities.Add(JsonConvert.DeserializeObject<T>(jsonEntity));
                        }
                    }
                    return entities;
                }
                catch (Exception ex)
                {
                    Logger.SaveLog(ex.Message);
                    return null;
                }
            }
            return null;
        }

        private string FormatServerName(string server)
        {
            int commaIndex = server.IndexOf(',');
            if (commaIndex >= 0)
            {
                return server.Substring(0, commaIndex);
            }
            return server;
        }
    }
}
