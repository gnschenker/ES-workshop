using System;
using Projects.Contracts.Commands;

namespace Projects.Domain
{
    public interface ISampleApplicationService
    {
        Guid When(StartSample cmd);
        void When(DoStep1 cmd);
        void When(ApproveSample cmd);
        void When(CancelSample cmd);
    }

    public class SampleApplicationService : ISampleApplicationService
    {
        private readonly IRepository _repository;
        public SampleApplicationService(IRepository repository)
        {
            _repository = repository;
        }

        public Guid When(StartSample cmd)
        {
            var id = Guid.NewGuid();
            Execute(id, aggregate => aggregate.Start(id, cmd.Name));
            return id;
        } 

        public void When(DoStep1 cmd)
        {
            Execute(cmd.SampleId, aggregate => aggregate.Step1(cmd.Quantity, cmd.DueDate));
        }

        public void When(ApproveSample cmd)
        {
            Execute(cmd.SampleId, aggregate => aggregate.Approve());
        }

        public void When(CancelSample cmd)
        {
            Execute(cmd.SampleId, aggregate => aggregate.Cancel());
        }

        private void Execute(Guid id, Action<SampleAggregate> action)
        {
            var aggregate = _repository.GetById<SampleAggregate>(id);
            action(aggregate);
            _repository.Save(aggregate);
        }
    }
}