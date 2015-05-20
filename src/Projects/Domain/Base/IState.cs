namespace Projects.Domain
{
    public interface IState
    {
        int Id { get; }
        int Version { get; }
        void Modify(object e);
    }
}