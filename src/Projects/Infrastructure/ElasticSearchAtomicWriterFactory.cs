namespace Projects.Infrastructure
{
    public class ElasticSearchAtomicWriterFactory : IProjectionWriterFactory
    {
        private readonly string _baseUrl;

        public ElasticSearchAtomicWriterFactory(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public IProjectionWriter<TView> GetProjectionWriter<TView>() where TView : class, new()
        {
            return new ElasticSearchProjectionWriter<TView>(_baseUrl);
        }
    }
}