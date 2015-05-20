using System;
using System.Threading.Tasks;

namespace Projects.Infrastructure
{
    public interface IProjectionWriter<TView> where TView : class
    {
        Task Add(int id, TView item);
        Task Update(int id, Action<TView> update);
    }
}