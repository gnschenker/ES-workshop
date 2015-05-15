using System.Collections.Generic;

namespace Projects.Domain
{
    public interface IAggregateFactory {
        T Create<T>(IEnumerable<object> events) where T : class, IAggregate;
    }
}