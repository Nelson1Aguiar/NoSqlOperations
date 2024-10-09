using Microsoft.Extensions.Configuration;
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
        private MongoClient _connectionMongoCliente;
        private readonly IConnectionNoSql _connectionNoSql;

        public MongoOperations(IConnectionNoSql connectionNoSql)
        {
            _connectionNoSql = connectionNoSql;
            _connectionMongoCliente = ConnectionNoSqlProvider.GetConnection(ConnectionTypeNoSql.MongoConnection, _connectionNoSql);
        }

        public async Task InsertInMongoAsync<T>(T entity, string collectionName)
        {
            string dataBaseName = ConnectionNoSqlProvider.GetConnection(ConnectionTypeNoSql.MongoDataBaseConnection, _connectionNoSql);
            var database = _connectionMongoCliente.GetDatabase(dataBaseName);
            if (_connectionMongoCliente != null)
            {
                try
                {
                    var collection = database.GetCollection<BsonDocument>(collectionName);
                    BsonDocument document = entity.ToBsonDocument();
                    await collection.InsertOneAsync(document);
                }
                catch (Exception ex)
                {
                    _connectionMongoCliente = null;
                }
            }
        }

        public List<T> GetInMongo<T>(FilterDefinition<T> filter, string collectionName)
        {
            string dataBaseName = ConnectionNoSqlProvider.GetConnection(ConnectionTypeNoSql.MongoDataBaseConnection, _connectionNoSql);
            var database = _connectionMongoCliente.GetDatabase(dataBaseName);

            if (_connectionMongoCliente != null)
            {
                try
                {
                    var collection = database.GetCollection<T>(collectionName);
                    Task<List<T>> task = collection.Find(filter).ToListAsync();
                    task.Wait();
                    return task.Result;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            return null;
        }

        public async Task UpdateInMongo<T>(FilterDefinition<T> filter, string collectionName, T updateDocument, params Expression<Func<T, object>>[] propertiesToUpdate)
        {
            string dataBaseName = ConnectionNoSqlProvider.GetConnection(ConnectionTypeNoSql.MongoDataBaseConnection, _connectionNoSql);
            var database = _connectionMongoCliente.GetDatabase(dataBaseName);
            if (_connectionMongoCliente != null)
            {
                try
                {
                    var collection = database.GetCollection<T>(collectionName);
                    var update = Builders<T>.Update.Combine(
                                             propertiesToUpdate.Select(prop => Builders<T>.Update.Set(prop, prop.Compile()(updateDocument))));

                    var result = await collection.UpdateOneAsync(filter, update);

                    if (result.ModifiedCount > 0)
                    {
                        Console.WriteLine("Documento atualizado com sucesso.");
                    }
                    else
                    {
                        Console.WriteLine("Nenhum documento foi atualizado.");
                    }

                }
                catch (Exception ex)
                {
                    //
                }
            }
        }

        public async Task DeleteInMongo<T>(FilterDefinition<T> filter, string collectionName)
        {
            string dataBaseName = ConnectionNoSqlProvider.GetConnection(ConnectionTypeNoSql.MongoDataBaseConnection, _connectionNoSql);
            var database = _connectionMongoCliente.GetDatabase(dataBaseName);
            if (_connectionMongoCliente != null)
            {
                try
                {
                    var collection = database.GetCollection<T>(collectionName);
                    var result = await collection.DeleteOneAsync(filter);
                }
                catch (Exception ex)
                {
                    _connectionMongoCliente = null;
                }
            }
        }

    }
}
