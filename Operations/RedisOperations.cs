using Newtonsoft.Json;
using NoSqlOperations.Enum;
using NoSqlOperations.Interfaces;
using StackExchange.Redis;

namespace NoSqlOperations.Operations
{
    public class RedisOperations : IRedisOperation
    {
        private readonly IConnectionRedis _connectionNoSql;

        public RedisOperations(IConnectionRedis connectionNoSql)
        {
            _connectionNoSql = connectionNoSql;
        }

        private IDatabase GetDatabase()
        {
            return _connectionNoSql.GenerateConnection(ConnectionTypeNoSql.RedisConnection);
        }

        private IServer GetServer(IDatabase db)
        {
            string serverName = db.Multiplexer.Configuration;
            string formattedServerName = FormatServerName(serverName);
            return db.Multiplexer.GetServer(formattedServerName);
        }

        public void SetData<T>(T entity, string redisKey)
        {
            try
            {
                IDatabase db = GetDatabase();
                string jsonEntity = JsonConvert.SerializeObject(entity);
                db.StringSet(redisKey, jsonEntity);
            }
            catch (Exception ex)
            {
                Logger.SaveLog(ex.Message);
            }
        }

        public T GetData<T>(string redisKey) where T : class, new()
        {
            try
            {
                IDatabase db = GetDatabase();
                string jsonEntity = db.StringGet(redisKey);
                return string.IsNullOrEmpty(jsonEntity) ? new T() : JsonConvert.DeserializeObject<T>(jsonEntity);
            }
            catch (Exception ex)
            {
                Logger.SaveLog(ex.Message);
                return new T();
            }
        }

        public List<T> GetAllDataByKeyPattern<T>(string redisKey)
        {
            List<T> entities = new List<T>();

            try
            {
                IDatabase db = GetDatabase();
                IServer server = GetServer(db);
                IEnumerable<RedisKey> keys = server.Keys(pattern: $"*{redisKey}*");

                foreach (RedisKey key in keys)
                {
                    string jsonEntity = db.StringGet(key);
                    if (!string.IsNullOrEmpty(jsonEntity))
                        entities.Add(JsonConvert.DeserializeObject<T>(jsonEntity));
                }
            }
            catch (Exception ex)
            {
                Logger.SaveLog(ex.Message);
            }

            return entities;
        }

        public void DeleteByKey(string redisKey)
        {
            try
            {
                IDatabase db = GetDatabase();
                db.KeyDelete(redisKey);
            }
            catch (Exception ex)
            {
                Logger.SaveLog(ex.Message, "DeleteByKey");
            }
        }

        public void DeleteAllByKeyPattern(string redisKey)
        {
            try
            {
                IDatabase db = GetDatabase();
                IServer server = GetServer(db);
                IEnumerable<RedisKey> keys = server.Keys(pattern: $"*{redisKey}*");

                foreach (RedisKey key in keys)
                {
                    db.KeyDelete(key);
                }
            }
            catch (Exception ex)
            {
                Logger.SaveLog(ex.Message, "DeleteAllByKeyPattern");
            }
        }

        private string FormatServerName(string server)
        {
            int commaIndex = server.IndexOf(',');
            return commaIndex >= 0 ? server.Substring(0, commaIndex) : server;
        }
    }
}
