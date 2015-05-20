using System;
using Projects.Contracts.Commands;

namespace Projects.Domain
{
    public interface ISampleApplicationService
    {
        int When(StartSample cmd);
        void When(DoStep1 cmd);
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

        public int When(StartSample cmd)
        {
            var id = _uniqueKeyGenerator.GetId<SampleAggregate>();
            Execute(id, aggregate => aggregate.Start(id, cmd.Name));
            return id;
        } 

        public void When(DoStep1 cmd)
        {
            Execute(cmd.Id, aggregate => aggregate.Step1(cmd.Quantity, cmd.DueDate));
        }

        private void Execute(int id, Action<SampleAggregate> action)
        {
            var aggregate = _repository.GetById<SampleAggregate>(id);
            action(aggregate);
            _repository.Save(aggregate);
        }
    }
}