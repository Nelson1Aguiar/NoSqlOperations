using MongoDB.Bson;
using MongoDB.Driver;
using NoSqlOperations.Enum;
using NoSqlOperations.Interfaces;
using System.Linq.Expressions;

namespace NoSqlOperations.Operations
{
    public class MongoOperations : IMongoOperations
    {
        private readonly MongoClient? _connectionMongoClient;
        private readonly IConnectionMongoDB _connectionNoSql;
        private readonly string? _mongoDataBaseName;
        private readonly IMongoDatabase? _mongoDatabase;

        public MongoOperations(IConnectionMongoDB connectionMongoDB)
        {
            _connectionNoSql = connectionMongoDB;
            _connectionMongoClient = _connectionNoSql.GenerateConnection(ConnectionTypeNoSql.MongoConnection);
            _mongoDataBaseName = _connectionNoSql.GetDataBase(ConnectionTypeNoSql.MongoDataBaseConnection);
            _mongoDatabase = _connectionMongoClient.GetDatabase(_mongoDataBaseName);
        }

        public async Task InsertInMongoAsync<T>(T entity, string collectionName)
        {
            if (_connectionMongoClient != null)
            {
                try
                {
                    IMongoCollection<BsonDocument> collection = _mongoDatabase.GetCollection<BsonDocument>(collectionName);
                    BsonDocument document = entity.ToBsonDocument();
                    await collection.InsertOneAsync(document);
                }
                catch (Exception ex)
                {
                    Logger.SaveLog(ex.Message);
                }
            }
        }

        public async Task<List<T>> GetListInMongoAsync<T>(Expression<Func<T, bool>> filterExpression, string collectionName)
        {
            if (_connectionMongoClient == null)
                return new List<T>();

            try
            {
                IMongoCollection<T> collection = _mongoDatabase.GetCollection<T>(collectionName);
                FilterDefinition<T> filter = Builders<T>.Filter.Where(filterExpression);
                return await collection.Find(filter)
                                       .Project<T>(Builders<T>.Projection.Exclude("_id"))
                                       .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.SaveLog(ex.Message);
                return new List<T>();
            }
        }

        public async Task<T> GetInMongoAsync<T>(Expression<Func<T, bool>> filterExpression, string collectionName) where T : class, new()
        {
            if (_connectionMongoClient == null)
                return new T();

            try
            {
                IMongoCollection<T> collection = _mongoDatabase.GetCollection<T>(collectionName);
                FilterDefinition<T> filter = Builders<T>.Filter.Where(filterExpression);
                return await collection.Find(filter)
                                       .Project<T>(Builders<T>.Projection.Exclude("_id"))
                                       .FirstAsync();
            }
            catch (Exception ex)
            {
                Logger.SaveLog(ex.Message);
                return new T();
            }
        }

        public List<T> GetListInMongo<T>(Expression<Func<T, bool>> filterExpression, string collectionName)
        {
            if (_connectionMongoClient == null)
                return new List<T>();

            try
            {
                IMongoCollection<T> collection = _mongoDatabase.GetCollection<T>(collectionName);
                FilterDefinition<T> filter = Builders<T>.Filter.Where(filterExpression);
                return collection.Find(filter)
                                 .Project<T>(Builders<T>.Projection.Exclude("_id"))
                                 .ToList();
            }
            catch (Exception ex)
            {
                Logger.SaveLog(ex.Message);
                return new List<T>();
            }
        }

        public T GetInMongo<T>(Expression<Func<T, bool>> filterExpression, string collectionName) where T : class, new()
        {
            if (_connectionMongoClient == null)
                new T();

            try
            {
                IMongoCollection<T> collection = _mongoDatabase.GetCollection<T>(collectionName);
                FilterDefinition<T> filter = Builders<T>.Filter.Where(filterExpression);
                return collection.Find(filter)
                                 .Project<T>(Builders<T>.Projection.Exclude("_id"))
                                 .First();
            }
            catch (Exception ex)
            {
                Logger.SaveLog(ex.Message);
                return new T();
            }
        }

        public async Task UpdateInMongoAsync<T>(Expression<Func<T, bool>> filterExpression, string collectionName, T updateDocument)
        {
            if (_connectionMongoClient != null)
            {
                try
                {
                    IMongoCollection<T> collection = _mongoDatabase.GetCollection<T>(collectionName);
                    FilterDefinition<T> filter = Builders<T>.Filter.Where(filterExpression);
                    var updateDefinitionList = typeof(T).GetProperties()
                                                        .Where(prop => prop.GetValue(updateDocument) != null &&
                                                        !(prop.PropertyType == typeof(string) && string.IsNullOrEmpty((string)prop.GetValue(updateDocument))))
                                                        .Select(prop => Builders<T>.Update.Set(prop.Name, prop.GetValue(updateDocument)))
                                                        .ToArray();


                    UpdateDefinition<T> update = Builders<T>.Update.Combine(updateDefinitionList);
                    UpdateResult result = await collection.UpdateManyAsync(filter, update);
                }
                catch (Exception ex)
                {
                    Logger.SaveLog(ex.Message);
                }
            }
        }


        public async Task DeleteInMongoAsync<T>(Expression<Func<T, bool>> filterExpression, string collectionName)
        {
            if (_connectionMongoClient != null)
            {
                try
                {
                    IMongoCollection<T> collection = _mongoDatabase.GetCollection<T>(collectionName);
                    FilterDefinition<T> filter = Builders<T>.Filter.Where(filterExpression);
                    DeleteResult result = await collection.DeleteManyAsync(filter);
                }
                catch (Exception ex)
                {
                    Logger.SaveLog(ex.Message);
                }
            }
        }
    }
}
