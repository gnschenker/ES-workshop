using System;
using System.Collections.Generic;

namespace SampleProject.Domain
{
    public interface IAggregate
    {
        Guid Id { get; }
        int Version { get; }
        IEnumerable<object> GetUncommittedEvents();
        void ClearUncommittedEvents();
    }
}