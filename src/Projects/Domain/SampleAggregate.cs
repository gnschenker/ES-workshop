using System;
using System.Collections.Generic;
using Projects.Contracts.Events;

namespace Projects.Domain
{
    public class SampleAggregate : AggregateBase<SampleAggregate>
    {
        public SampleAggregate(IEnumerable<object> events) : base(events)
        { }

        public void Start(int id, string name)
        {
            if(Version>0)
                throw new InvalidOperationException("Cannot start already started sample");

            Apply(new SampleStarted{Id = id, Name = name});
        }

        public void Step1(int quantity)
        {
            if (Version == 0)
                throw new InvalidOperationException("Cannot execute step 1 on sample that is not started");

            Apply(new Step1Executed { Id = Id, Quantity = quantity });
        }

        private void When(SampleStarted e)
        {
            Id = e.Id;
        }
    }
}