using System;
using System.Threading.Tasks;

namespace Projects.Infrastructure
{
    public interface IProvider<T>
    {
        Task<T> GetById(Guid id);
    }
}