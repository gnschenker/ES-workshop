using System;
using System.Threading.Tasks;

namespace SampleProject.Infrastructure
{
    public interface IProjectionWriter<in TId, TView> where TView : class
    {
        Task<TView> AddOrUpdate(TId key, Func<TView> addFactory, Func<TView, TView> update, bool probablyExists = true);
    }
}