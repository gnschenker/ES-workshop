using System;

namespace Projects.Domain
{
    public interface IRepository
    {
        T GetById<T>(Guid id) where T : class, IAggregate;
        void Save(IAggregate aggregate);
    }
}