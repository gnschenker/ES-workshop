using System;
using System.Threading.Tasks;

namespace SampleProject.Infrastructure
{
    public interface IProjectionWriter<in TId, TView> where TView : class
    {
        Task Add(TId id, TView item);
        Task Update(TId id, Action<TView> update);
    }
}