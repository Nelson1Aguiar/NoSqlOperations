namespace NoSqlOperations.Interfaces
{
    public interface IRedisOperation
    {
        public void SetData<T>(T entity, string redisKey);
        public T GetData<T>(string redisKey) where T : class, new();
        public List<T> GetAllDataByKey<T>(string redisKey);
    }
}
