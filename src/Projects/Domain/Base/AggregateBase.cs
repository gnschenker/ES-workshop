using System.Collections.Generic;

namespace Projects.Domain
{
    public abstract class AggregateBase<T> : IAggregate
        where T : AggregateBase<T>
    {
        private readonly IList<object> _uncommitedEvents = new List<object>();

        protected AggregateBase(IEnumerable<object> events = null)
        {
            if (events == null) return;
            foreach (var @event in events)
                InternalApply(@event);
        }

        public IEnumerable<object> GetUncommittedEvents()
        {
            return _uncommitedEvents;
        }

        public void ClearUncommittedEvents()
        {
            _uncommitedEvents.Clear();
        }

        protected void AddUncommitedEvent(object e)
        {
            _uncommitedEvents.Add(e);
        }

        protected void Apply(object e)
        {
            _uncommitedEvents.Add(e);
            InternalApply(e);
        }

        private void InternalApply(object e)
        {
            Version++;
            RedirectToWhen.InvokeEventOptional((T)this, e);
        }

        public int Id { get; protected set; }
        public int Version { get; private set; }
    }
}