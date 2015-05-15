using System.Collections.Generic;

namespace Projects.Domain
{
    public interface IAggregate
    {
        int Id { get; }
        int Version { get; }
        IEnumerable<object> GetUncommittedEvents();
        void ClearUncommittedEvents();
    }
}