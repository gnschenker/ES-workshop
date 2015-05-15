namespace Projects.Domain
{
    public interface IRepository
    {
        T GetById<T>(int id) where T : class, IAggregate;
        void Save(IAggregate aggregate);
    }
}