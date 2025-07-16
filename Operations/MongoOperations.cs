using MongoDB.Bson;
using MongoDB.Driver;
using NoSqlOperations.Enum;
using NoSqlOperations.Interfaces;
using System.Linq.Expressions;

namespace NoSqlOperations.Operations
{
    public class MongoOperations : IMongoOperations
    {
        private readonly IConnectionMongoDB _connectionMongoDB;

        public MongoOperations(IConnectionMongoDB connectionMongoDB)
        {
            _connectionMongoDB = connectionMongoDB;
        }

        private IMongoDatabase GetDatabase()
        {
            MongoClient client = _connectionMongoDB.GenerateConnection(ConnectionTypeNoSql.MongoConnection);
            string dbName = _connectionMongoDB.GetDataBase(ConnectionTypeNoSql.MongoDataBaseConnection);
            return client?.GetDatabase(dbName) ?? throw new Exception("MongoDB connection failed.");
        }

        public async Task InsertInMongoAsync<T>(T entity, string collectionName)
        {
            try
            {
                IMongoDatabase db = GetDatabase();
                IMongoCollection<BsonDocument> collection = db.GetCollection<BsonDocument>(collectionName);
                BsonDocument document = entity.ToBsonDocument();
                await collection.InsertOneAsync(document);
            }
            catch (Exception ex)
            {
                Logger.SaveLog(ex.Message);
            }
        }

        public async Task<List<T>> GetListInMongoAsync<T>(Expression<Func<T, bool>> filterExpression, string collectionName)
        {
            try
            {
                IMongoDatabase db = GetDatabase();
                IMongoCollection<T> collection = db.GetCollection<T>(collectionName);
                var filter = Builders<T>.Filter.Where(filterExpression);
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
            try
            {
                IMongoDatabase db = GetDatabase();
                IMongoCollection<T> collection = db.GetCollection<T>(collectionName);
                var filter = Builders<T>.Filter.Where(filterExpression);
                return await collection.Find(filter)
                                       .Project<T>(Builders<T>.Projection.Exclude("_id"))
                                       .FirstOrDefaultAsync() ?? new T();
            }
            catch (Exception ex)
            {
                Logger.SaveLog(ex.Message);
                return new T();
            }
        }

        public List<T> GetListInMongo<T>(Expression<Func<T, bool>> filterExpression, string collectionName)
        {
            try
            {
                IMongoDatabase db = GetDatabase();
                IMongoCollection<T> collection = db.GetCollection<T>(collectionName);
                var filter = Builders<T>.Filter.Where(filterExpression);
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
            try
            {
                IMongoDatabase db = GetDatabase();
                IMongoCollection<T> collection = db.GetCollection<T>(collectionName);
                var filter = Builders<T>.Filter.Where(filterExpression);
                return collection.Find(filter)
                                 .Project<T>(Builders<T>.Projection.Exclude("_id"))
                                 .FirstOrDefault() ?? new T();
            }
            catch (Exception ex)
            {
                Logger.SaveLog(ex.Message);
                return new T();
            }
        }

        public async Task UpdateInMongoAsync<T>(Expression<Func<T, bool>> filterExpression, string collectionName, T updateDocument)
        {
            try
            {
                IMongoDatabase db = GetDatabase();
                IMongoCollection<T> collection = db.GetCollection<T>(collectionName);
                var filter = Builders<T>.Filter.Where(filterExpression);

                var updateDefinitionList = typeof(T).GetProperties()
                    .Where(prop => prop.GetValue(updateDocument) != null &&
                                  !(prop.PropertyType == typeof(string) && string.IsNullOrEmpty((string)prop.GetValue(updateDocument))))
                    .Select(prop => Builders<T>.Update.Set(prop.Name, prop.GetValue(updateDocument)))
                    .ToArray();

                var update = Builders<T>.Update.Combine(updateDefinitionList);
                await collection.UpdateManyAsync(filter, update);
            }
            catch (Exception ex)
            {
                Logger.SaveLog(ex.Message);
            }
        }


        public async Task DeleteInMongoAsync<T>(Expression<Func<T, bool>> filterExpression, string collectionName)
        {
            try
            {
                IMongoDatabase db = GetDatabase();
                IMongoCollection<T> collection = db.GetCollection<T>(collectionName);
                var filter = Builders<T>.Filter.Where(filterExpression);
                await collection.DeleteManyAsync(filter);
            }
            catch (Exception ex)
            {
                Logger.SaveLog(ex.Message);
            }
        }
    }
}
