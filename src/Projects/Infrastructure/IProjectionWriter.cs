using System;
using System.Threading.Tasks;

namespace Projects.Infrastructure
{
    public interface IProjectionWriter<TView> where TView : class
    {
        Task Add(Guid id, TView item);
        Task Update(Guid id, Action<TView> update);
    }
}