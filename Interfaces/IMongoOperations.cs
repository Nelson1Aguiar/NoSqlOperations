using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NoSqlOperations.Interfaces
{
    public interface IMongoOperations
    {
        public void InsertInMongoAsync<T>(T entity, string collectionName);
        public List<T>? GetInMongo<T>(FilterDefinition<T> filter, string collectionName);
        public void UpdateInMongoAsync<T>(FilterDefinition<T> filter, string collectionName, T updateDocument, params Expression<Func<T, object>>[] propertiesToUpdate);
        public void DeleteInMongoAsync<T>(FilterDefinition<T> filter, string collectionName);

    }
}
