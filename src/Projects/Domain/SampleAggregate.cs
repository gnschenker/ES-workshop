using System;
using Projects.Contracts.Events;

namespace Projects.Domain
{
    public class SampleAggregate : AggregateBase<SampleState>
    {
        public SampleAggregate(SampleState state) : base(state)
        { }

        public void Start(int id, string name)
        {
            if(State.Version>0)
                throw new InvalidOperationException("Cannot start already started sample");

            Apply(new SampleStarted{Id = id, Name = name});
        }

        public void Step1(int quantity, DateTime dueDate)
        {
            if (State.Version == 0)
                throw new InvalidOperationException("Cannot execute step 1 on sample that is not started");

            Apply(new Step1Executed
            {
                Id = State.Id,
                Quantity = quantity,
                DueDate = dueDate
            });
        }
    }
}