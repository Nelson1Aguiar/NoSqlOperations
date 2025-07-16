namespace NoSqlOperations.Interfaces
{
    public interface IRedisOperation
    {
        public void SetData<T>(T entity, string redisKey);
        public T GetData<T>(string redisKey) where T : class, new();
        public List<T> GetAllDataByKeyPattern<T>(string redisKey);
        public void DeleteByKey(string redisKey);
        public void DeleteAllByKeyPattern(string redisKey);
    }
}
