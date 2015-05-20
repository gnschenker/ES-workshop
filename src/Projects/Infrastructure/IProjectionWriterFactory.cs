namespace Projects.Infrastructure
{
    public interface IProjectionWriterFactory
    {
        IProjectionWriter<TView> GetProjectionWriter<TView>() where TView : class, new();
    }
}