using System;
using System.Collections.Generic;
using Projects.Contracts.Events;

namespace Projects.Domain
{
    public class SampleState : IState
    {
        public Guid Id { get; private set; }
        public int Version { get; private set; }
        public SampleStatus Status { get; set; }

        public SampleState(IEnumerable<object> events)
        {
            if (events == null) return;
            foreach (var @event in events)
                Modify(@event);
        }

        private void When(SampleStarted e)
        {
            Id = e.Id;
            Status = SampleStatus.Draft;
        }

        private void When(SampleApproved e)
        {
            Status = SampleStatus.Approved;
        }

        private void When(SampleCancelled e)
        {
            Status = SampleStatus.Cancelled;
        }

        public void Modify(object @event)
        {
            Version++;
            RedirectToWhen.InvokeEventOptional(this, @event);
        }
    }

    public enum SampleStatus
    {
        Undefined = 0,
        Draft = 1,
        Approved = 2,
        Withdrawn = 3,
        Cancelled
    }
}