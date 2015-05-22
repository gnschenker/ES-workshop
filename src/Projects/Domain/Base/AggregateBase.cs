using System;
using System.Collections.Generic;

namespace Projects.Domain
{
    public abstract class AggregateBase<TState> : IAggregate
        where TState : IState
    {
        private readonly IList<object> _uncommitedEvents = new List<object>();
        protected TState State;

        protected AggregateBase(TState state)
        {
            State = state;
        }

        protected void AddUncommitedEvent(object e)
        {
            _uncommitedEvents.Add(e);
        }

        protected void Apply(object e)
        {
            _uncommitedEvents.Add(e);
            State.Modify(e);
        }

        IEnumerable<object> IAggregate.GetUncommittedEvents()
        {
            return _uncommitedEvents;
        }

        void IAggregate.ClearUncommittedEvents()
        {
            _uncommitedEvents.Clear();
        }

        Guid IAggregate.Id { get { return State.Id; } }
        int IAggregate.Version { get { return State.Version; } }
    }
}