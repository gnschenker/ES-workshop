namespace Projects.Domain
{
    public interface IUniqueKeyGenerator
    {
        int GetId<T>();
    }
}