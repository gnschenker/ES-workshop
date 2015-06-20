namespace SampleProject.Infrastructure
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

        public IProjectionWriter<TId, TView> GetProjectionWriter<TId, TView>() where TView : class, new()
        {
            return new MongoDbProjectionWriter<TId, TView>(_connectionString, _dbName);
        }
    }
}