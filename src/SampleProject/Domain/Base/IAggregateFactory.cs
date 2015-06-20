using System.Collections.Generic;

namespace SampleProject.Domain
{
    public interface IAggregateFactory {
        T Create<T>(IEnumerable<object> events) where T : class, IAggregate;
    }
}