namespace SampleProject.Infrastructure
{
    public class SqlServerAtomicWriterFactory : IProjectionWriterFactory
    {
        private readonly string _connectionString;
        public SqlServerAtomicWriterFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IProjectionWriter<TId, TView> GetProjectionWriter<TId, TView>() where TView : class, new()
        {
            return new SqlServerProjectionWriter<TId, TView>(_connectionString);
        }
    }
}