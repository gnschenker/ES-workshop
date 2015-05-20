namespace Projects.Infrastructure
{
    public class MongoDbAtomicWriterFactory : IProjectionWriterFactory
    {
        private readonly string _connectionString;
        private readonly string _dbName;

        public MongoDbAtomicWriterFactory(string connectionString, string dbName)
        {
            _connectionString = connectionString;
            _dbName = dbName;
        }

        public IProjectionWriter<TView> GetProjectionWriter<TView>() where TView : class, new()
        {
            return new MongoDbProjectionWriter<TView>(_connectionString, _dbName);
        }
    }
}