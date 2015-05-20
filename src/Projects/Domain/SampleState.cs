using System.Collections.Generic;
using Projects.Contracts.Events;

namespace Projects.Domain
{
    public class SampleState : IState
    {
        public int Id { get; private set; }
        public int Version { get; private set; }

        public SampleState(IEnumerable<object> events)
        {
            if (events == null) return;
            foreach (var @event in events)
                Modify(@event);
        }

        private void When(SampleStarted e)
        {
            Id = e.Id;
        }

        public void Modify(object @event)
        {
            Version++;
            RedirectToWhen.InvokeEventOptional(this, @event);
        }
    }
}