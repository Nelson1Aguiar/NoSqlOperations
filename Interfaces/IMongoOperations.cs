using System.Linq.Expressions;

namespace NoSqlOperations.Interfaces
{
    public interface IMongoOperations
    {
        public Task InsertInMongoAsync<T>(T entity, string collectionName);
        public Task<List<T>> GetListInMongoAsync<T>(Expression<Func<T, bool>> filterExpression, string collectionName);
        public List<T> GetListInMongo<T>(Expression<Func<T, bool>> filterExpression, string collectionName);
        public Task<T> GetInMongoAsync<T>(Expression<Func<T, bool>> filterExpression, string collectionName) where T : class, new();
        public T GetInMongo<T>(Expression<Func<T, bool>> filterExpression, string collectionName) where T : class, new();
        public Task UpdateInMongoAsync<T>(Expression<Func<T, bool>> filterExpression, string collectionName, T updateDocument);
        public Task DeleteInMongoAsync<T>(Expression<Func<T, bool>> filterExpression, string collectionName);

    }
}
