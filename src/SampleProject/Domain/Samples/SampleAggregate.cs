using System;
using SampleProject.Contracts.Events;

namespace SampleProject.Domain.Samples
{
    public class SampleAggregate : AggregateBase<SampleState>
    {
        public SampleAggregate(SampleState state) : base(state)
        { }

        public void Start(Guid id, string name)
        {
            if(State.Version>0)
                throw new InvalidOperationException("Cannot start already started sample");

            Apply(new SampleStarted{Id = id, Name = name});
        }

        public void Step1(int quantity, DateTime dueDate)
        {
            if (State.Version == 0)
                throw new InvalidOperationException("Cannot execute step 1 on sample that is not started");
            if (State.Status != SampleStatus.Draft)
                throw new InvalidOperationException(string.Format("Cannot execute step 1 on sample that is in the following status: {0}", State.Status));

            Apply(new Step1Executed
            {
                Id = State.Id,
                Quantity = quantity,
                DueDate = dueDate
            });
        }

        public void Approve()
        {
            if (State.Status != SampleStatus.Draft)
                throw new InvalidOperationException(string.Format("Cannot approve a sample that is in the following status: {0}", State.Status));
            
            Apply(new SampleApproved
            {
                Id = State.Id
            });
        }

        public void Cancel()
        {
            Apply(new SampleCancelled{Id = State.Id});
        }
    }
}