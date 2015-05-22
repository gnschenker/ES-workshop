using System;
using Projects.Contracts.Commands;

namespace Projects.Domain
{
    public interface ISampleApplicationService
    {
        Guid When(StartSample cmd);
        void When(DoStep1 cmd);
        void When(ApproveSample cmd);
    }

    public class SampleApplicationService : ISampleApplicationService
    {
        private readonly IRepository _repository;
        private readonly IUniqueKeyGenerator _uniqueKeyGenerator;

        public SampleApplicationService(IRepository repository, IUniqueKeyGenerator uniqueKeyGenerator)
        {
            _repository = repository;
            _uniqueKeyGenerator = uniqueKeyGenerator;
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

        private void Execute(Guid id, Action<SampleAggregate> action)
        {
            var aggregate = _repository.GetById<SampleAggregate>(id);
            action(aggregate);
            _repository.Save(aggregate);
        }
    }
}