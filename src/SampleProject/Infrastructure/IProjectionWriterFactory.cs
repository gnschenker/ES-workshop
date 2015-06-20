namespace SampleProject.Infrastructure
{
    public interface IProjectionWriterFactory
    {
        IProjectionWriter<TId, TView> GetProjectionWriter<TId, TView>() where TView : class, new();
    }
}