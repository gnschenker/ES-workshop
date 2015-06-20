namespace SampleProject.Infrastructure
{
    public class SqlServerAtomicWriterFactory : IProjectionWriterFactory
    {
        private readonly string _connectionString;
        public SqlServerAtomicWriterFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IProjectionWriter<TView> GetProjectionWriter<TView>() where TView : class, new()
        {
            return new SqlServerProjectionWriter<TView>(_connectionString);
        }
    }
}