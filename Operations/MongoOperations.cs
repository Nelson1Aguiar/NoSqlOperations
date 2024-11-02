using MongoDB.Bson;
using MongoDB.Driver;
using NoSqlOperations.Connector;
using NoSqlOperations.Enum;
using NoSqlOperations.Interfaces;
using System.Linq.Expressions;

namespace NoSqlOperations.Operations
{
    public class MongoOperations : IMongoOperations
    {
        private readonly MongoClient _connectionMongoClient;
        private readonly IConnectionNoSql _connectionNoSql;
        private readonly string _mongoDataBaseName;
        private readonly IMongoDatabase _mongoDatabase;

        public MongoOperations(IConnectionNoSql connectionNoSql)
        {
            _connectionNoSql = connectionNoSql;
            _connectionMongoClient = _connectionNoSql.GenerateConnection(ConnectionTypeNoSql.MongoConnection);
            _mongoDataBaseName = _connectionNoSql.GenerateConnection(ConnectionTypeNoSql.MongoDataBaseConnection);
            _mongoDatabase = _connectionMongoClient.GetDatabase(_mongoDataBaseName);
        }

        public async void InsertInMongoAsync<T>(T entity, string collectionName)
        {
            if (_connectionMongoClient != null)
            {
                try
                {
                    var collection = _mongoDatabase.GetCollection<BsonDocument>(collectionName);
                    BsonDocument document = entity.ToBsonDocument();
                    await collection.InsertOneAsync(document);
                }
                catch (Exception ex)
                {
                    Logger.SaveLog(ex.Message);
                }
            }
        }

        public List<T>? GetInMongo<T>(FilterDefinition<T> filter, string collectionName)
        {
            if (_connectionMongoClient != null)
            {
                try
                {
                    var collection = _mongoDatabase.GetCollection<T>(collectionName);
                    Task<List<T>> task = collection.Find(filter).ToListAsync();
                    task.Wait();
                    return task.Result;
                }
                catch (Exception ex)
                {
                    Logger.SaveLog(ex.Message);
                    return null;
                }
            }
            return null;
        }

        public async void UpdateInMongoAsync<T>(FilterDefinition<T> filter, string collectionName, T updateDocument, params Expression<Func<T, object>>[] propertiesToUpdate)
        {
            if (_connectionMongoClient != null)
            {
                try
                {
                    var collection = _mongoDatabase.GetCollection<T>(collectionName);
                    var update = Builders<T>.Update.Combine(
                                             propertiesToUpdate.Select(prop => Builders<T>.Update.Set(prop, prop.Compile()(updateDocument))));

                    var result = await collection.UpdateOneAsync(filter, update);
                }
                catch (Exception ex)
                {
                    Logger.SaveLog(ex.Message);
                }
            }
        }

        public async void DeleteInMongoAsync<T>(FilterDefinition<T> filter, string collectionName)
        {
            if (_connectionMongoClient != null)
            {
                try
                {
                    var collection = _mongoDatabase.GetCollection<T>(collectionName);
                    var result = await collection.DeleteOneAsync(filter);
                }
                catch (Exception ex)
                {
                    Logger.SaveLog(ex.Message);
                }
            }
        }
    }
}
