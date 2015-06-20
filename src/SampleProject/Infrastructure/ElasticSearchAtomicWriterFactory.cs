namespace SampleProject.Infrastructure
{
    public class ElasticSearchAtomicWriterFactory : IProjectionWriterFactory
    {
        private readonly string _baseUrl;

        public ElasticSearchAtomicWriterFactory(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public IProjectionWriter<TId, TView> GetProjectionWriter<TId, TView>() where TView : class, new()
        {
            return new ElasticSearchProjectionWriter<TId, TView>(_baseUrl);
        }
    }
}