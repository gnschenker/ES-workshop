using System;
using System.Collections.Generic;

namespace Projects.Domain
{
    public class AggregateFactory : IAggregateFactory
    {
        public T Create<T>(IEnumerable<object> events) where T : class, IAggregate
        {
            if (typeof (T) == typeof (SampleAggregate))
                return new SampleAggregate(events) as T;

            throw new ArgumentException("Unknown aggregate type");
        }
    }
}