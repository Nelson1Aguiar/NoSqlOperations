using Newtonsoft.Json;
using NoSqlOperations.Enum;
using NoSqlOperations.Interfaces;
using StackExchange.Redis;

namespace NoSqlOperations.Operations
{
    public class RedisOperations : IRedisOperation
    {
        private IDatabase? _dataBase;
        private readonly IConnectionRedis _connectionNoSql;

        public RedisOperations(IConnectionRedis connectionNoSql)
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

        public T GetData<T>(string redisKey) where T : class, new()
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
                    return new T();
                }
            }
            return new T();
        }

        public List<T> GetAllDataByKeyPattern<T>(string redisKey)
        {
            List<T> entities = new List<T>();

            if (_dataBase != null)
            {
                try
                {
                    string pattern = $"*{redisKey}*";
                    string serverName = _dataBase.Multiplexer.Configuration;
                    string formatServerName = FormatServerName(serverName);
                    IServer server = _dataBase.Multiplexer.GetServer(formatServerName);
                    IEnumerable<RedisKey> keys = server.Keys(pattern: pattern);
                    foreach (RedisKey key in keys)
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
                    return entities;
                }
            }
            return entities;
        }

        public void DeleteByKey(string redisKey)
        {
            if (_dataBase != null)
            {
                try
                {
                    _dataBase.KeyDelete(redisKey);
                }
                catch (Exception ex)
                {
                    Logger.SaveLog(ex.Message, "DeleteByKey");
                }
            }
        }

        public void DeleteAllByKeyPattern(string redisKey)
        {
            if (_dataBase != null)
            {
                try
                {
                    string pattern = $"*{redisKey}*";
                    string serverName = _dataBase.Multiplexer.Configuration;
                    string formattedServerName = FormatServerName(serverName);
                    IServer server = _dataBase.Multiplexer.GetServer(formattedServerName);

                    IEnumerable<RedisKey> keys = server.Keys(pattern: pattern);

                    foreach (RedisKey key in keys)
                    {
                        _dataBase.KeyDelete(key);
                    }
                }
                catch (Exception ex)
                {
                    Logger.SaveLog(ex.Message, "DeleteAllByKeyPattern");
                }
            }
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
