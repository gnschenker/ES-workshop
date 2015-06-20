namespace SampleProject.Domain
{
    public interface IUniqueKeyGenerator
    {
        int GetId<T>();
    }
}